using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Diagnostics;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Net;
using System.IO;
using System.Data;
using System.Windows;

namespace TapTrack.Demo
{
    class DatabaseUtility
    {
        private static DataContext dc = new DataContext(Properties.Settings.Default.NDEFBatchMessagesDBConnectionString);

        public static bool isOpen()
        {
            if (dc.Connection.State.HasFlag(System.Data.ConnectionState.Open))
                return true;
            else
                return false;
        }

        public static bool Connect()
        {
            try
            {
                dc.Connection.Open();
                return isOpen();
            }
            catch (Exception e)
            {
                return false;
            }


        }

        public static bool InsertNDEFMessagesToEncodeSQL(List<BatchNDEF> messagesToInsert)
        {
            int numRows;
            int totalRowsAdded = 0;
            foreach (BatchNDEF message in messagesToInsert)
            {
                try
                {
                    StringBuilder sb = new StringBuilder(@"INSERT INTO messageToEncode VALUES('");
                    sb.Append(message.type);
                    sb.Append("','");
                    sb.Append(message.data);
                    sb.Append("','");
                    sb.Append(message.encodedSuccessfully);
                    sb.Append("')");

                    string sqlCommand = sb.ToString();

                    using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.NDEFBatchMessagesDBConnectionString))
                    {
                        SqlCommand command = new SqlCommand(sqlCommand, connection);
                        connection.Open();
                        numRows = command.ExecuteNonQuery();
                        totalRowsAdded++;
                        Console.WriteLine(numRows + " record has been inserted into messagesToEncode table, total rows added this import is " + totalRowsAdded);                 
                        command.Connection.Close();
                    }                
                }
                catch(Exception exc)
                {
                    return false;
                }              
            }
            return true;
        }

        public static bool InsertEncodingEventSQL(EncodingEvent encodingEvent)
        {
            int numRows;

            try
                {
                    using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.NDEFBatchMessagesDBConnectionString))
                    {
                        SqlCommand command = new SqlCommand("INSERT INTO encodingEvent (tagCode, type, data, encodedDate) VALUES(@tagCode,@type,@data,@encodedDate)", connection);
                        command.Parameters.AddWithValue("tagCode", encodingEvent.tagCode);
                        command.Parameters.AddWithValue("type", encodingEvent.type);
                        command.Parameters.AddWithValue("data", encodingEvent.data);
                        command.Parameters.AddWithValue("encodedDate", encodingEvent.encodedDate);
                        connection.Open();
                        numRows = command.ExecuteNonQuery();    
                        Console.WriteLine(numRows + " record(s) has been inserted into encodingEvent table");
                        command.Connection.Close();
                    }
                }
                catch (Exception exc)
                {
                    return false;
                }
               return true;
        }

        public static List<BatchNDEF> SelectCurrentBatch()
        {
            if (isOpen() == false)
            {
                if (Connect() == false)
                {
                    return null;
                }
            }

            Table<BatchNDEF> allImportedMessages = dc.GetTable<BatchNDEF>();
            dc.Refresh(RefreshMode.KeepChanges, allImportedMessages);

            List<BatchNDEF> l = (from c in allImportedMessages where c.encodedSuccessfully == 0 select c).ToList();

            return l;
        }

        public static List<BatchNDEF> SelectAllImportedBatch()
        {
            if (isOpen() == false)
            {
                if (Connect() == false)
                {
                    return null;
                }
            }
           
            Table<BatchNDEF> allImportedMessages = dc.GetTable<BatchNDEF>();
            dc.Refresh(RefreshMode.KeepChanges, allImportedMessages);

            List<BatchNDEF> l = (from c in allImportedMessages select c).ToList();

            return l;
        }

        public static int GetNumberOfUnencodedTagsInCurrentBatch()
        {
            if(SelectCurrentBatch() != null)
            {
                return SelectCurrentBatch().Count;
            }
            else
            {
                throw new Exception("An error occured when selecting from the messageToEncode table");
            }
            
        }


        public static bool UpdateMessagesToEncodedSQL(int messageId)
        {
            if (isOpen() == false)
            {
                if (Connect() == false)
                {
                    return false;
                }
            }

            try
            {
                StringBuilder sb = new StringBuilder(@"UPDATE messageToEncode SET encodedSuccessfully = '1' WHERE id = ");
                sb.Append(messageId.ToString());
                int numRows = 0;

                string sqlCommand = sb.ToString();

                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.NDEFBatchMessagesDBConnectionString))
                {
                    SqlCommand command = new SqlCommand(sqlCommand, connection);
                    connection.Open();
                    numRows = command.ExecuteNonQuery();
                    command.Connection.Close();
                    if (numRows != 0)
                    {
                        Console.WriteLine(numRows + " record has been updated to encoded successfully in the messagesToEncode table");
                    }
                    else
                    {
                        return false;
                    }                                       
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool ClearImportedBatchTable()
        {
            if (isOpen() == false)
            {
                if (Connect() == false)
                {
                    return false;
                }
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(Properties.Settings.Default.NDEFBatchMessagesDBConnectionString))
                {
                    SqlCommand command = new SqlCommand("DELETE messageToEncode", connection);
                    connection.Open();
                    command.ExecuteNonQuery();
                    command.Connection.Close();
                }
            }
            catch (Exception exc)
            {
                return false;
            }
            return true;

        }

        public static List<EncodingEvent> SelectAllEncodingEvents()
        {
            if (isOpen() == false)
            {
                if (Connect() == false)
                {
                    return null;
                }
            }

            Table<EncodingEvent> allEncodingEvents = dc.GetTable<EncodingEvent>();
            dc.Refresh(RefreshMode.KeepChanges, allEncodingEvents);

            List<EncodingEvent> l = (from c in allEncodingEvents select c).ToList();

            return l;
        }


    }
}
