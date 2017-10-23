using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Comms;
using Logger;
using Microsoft.Data.Sqlite;

namespace AMLWorker
{
    public class FuzzyMatcher : FuzzyMatcherServer
    {
        string FuzzyPhraseCreate = "create table FuzzyPhrase (phrase text,unique(phrase));";
        string FuzzyTripleCreate = "create virtual table FuzzyTriple using fts4(triple);";
        string FuzzyPhraseToDocument = "create table FuzzyPhraseToDocument (phraseid integer, documentid integer, primary key(phraseid,documentid), foreign key (phraseid) references FuzzyPhrase(phrase));";

        private string connectionString;
        public FuzzyMatcher(IServiceServer server) : base(server)
        {
            L.Trace(
                $"Opened server with bucket {server.BucketId} and data dir - {server.GetConfigProperty("DataDirectory")}");

            connectionString = (string)server.GetConfigProperty("DataDirectory") +
                                   $"/FuzzyMatcher_{server.BucketId}";
            L.Trace($"Initializing Sql - connectionString is {connectionString}");

            using (var connection = newConnection())
            {
                connection.Open();

                if (!SqlHelper.TableExists(connection,"FuzzyPhrase"))
                {
                    SqlHelper.ExecuteCommandLog(connection, FuzzyPhraseCreate);
                    SqlHelper.ExecuteCommandLog(connection,FuzzyTripleCreate);
                    SqlHelper.ExecuteCommandLog(connection, FuzzyPhraseToDocument);

                }
            }
        }

        SqliteConnection newConnection()
        {
            return new SqliteConnection(
                "" + new SqliteConnectionStringBuilder {DataSource = $"{connectionString}"});
        }


        public override bool AddEntry(List<FuzzyWordEntry> entries)
        {
            using (var connection = newConnection())
            {
                var txn = connection.BeginTransaction();

                var existsCmd = connection.CreateCommand();
                existsCmd.CommandText = "select rowid from FuzzyPhrase where Phrase=($phrase)";

                var insert3Cmd = connection.CreateCommand();
                insert3Cmd.Transaction = txn;
                insert3Cmd.CommandText = 
                    $@"insert into FuzzyPhrase (phrase) values ($phrase);
                    insert into FuzzyTriple (triple) values ($triple);
                    insert into FuzzyPhraseToDocument (phraseid,documentid) values (last_insert_rowid(),$documentid);";

                var insert1Cmd = connection.CreateCommand();
                insert1Cmd.Transaction = txn;
                insert1Cmd.CommandText =
                    "insert into FuzzyPhraseToDocument (phraseid,documentid) values ($phraseid,$documentid);";


                foreach (var c in entries)
                {
                    existsCmd.Parameters.AddWithValue("$phrase", c.Phrase);
                    var exists = existsCmd.ExecuteReader();
                    if (exists.HasRows)
                    {
                        insert1Cmd.Parameters.AddWithValue("$phraseid", exists.GetInt32(0));
                        insert1Cmd.Parameters.AddWithValue("$documentid", c.DocId);
                    }
                    else
                    {
                        insert3Cmd.Parameters.AddWithValue("$phrase", c.Phrase);
                        insert3Cmd.Parameters.AddWithValue("$documentid", c.DocId);
                        insert3Cmd.Parameters.AddWithValue("$triple", CreateTriple(c.Phrase));

                        var row = insert3Cmd.ExecuteNonQuery();
                        if (row != 2)
                            throw new Exception($"Could not insert row .. {c.Phrase}");
                    }

                }

                txn.Commit();


            }
            return true;
        }

        String CreateTriple(String phrase)
        {
            StringBuilder b = new StringBuilder();
            int tripleChars = 0;
            for (int cnt = 0; cnt < phrase.Length; cnt++,tripleChars++)
            {
                char z = phrase[cnt];
                if (z == ' ')
                    z = '_';
                b.Append(z);
                if (tripleChars > 0 && tripleChars % 3 == 0)
                {
                    b.Append(' ');
                    cnt--;
                }
            }
            return b.ToString();
        }
    }
}
