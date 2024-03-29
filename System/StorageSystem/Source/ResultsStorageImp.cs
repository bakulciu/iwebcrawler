﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using CrawlerNameSpace.Utilities;

namespace CrawlerNameSpace.StorageSystem
{
    public class ResultsStorageImp : ResultsStorage
    {
        //this is a private method that compares between two result object according to their rank.
        //this method is used as delegate function in the sort of list.
        private static int CompareResultByRank(Result res1, Result res2)
        {
            if (res1 == null)
            {
                if (res2 == null)
                    return 0;
                else
                    return -1;
            }
            else
                if (res2 == null)
                    return 1;
                else
                {
                    if (res1.getRank() < res2.getRank())
                        return -1;
                    else
                        if (res1.getRank() == res2.getRank())
                            return 0;
                        else
                            return 1;
                }
        }

        //this is a private method that compares between two result object according to their trust meter.
        //this method is used as delegate function in the sort of list.
        private static int CompareResultByTrustMeter(Result res1, Result res2)
        {
            if (res1 == null)
            {
                if (res2 == null)
                    return 0;
                else
                    return -1;
            }
            else
                if (res2 == null)
                    return 1;
                else
                {
                    if (res1.getTrustMeter() < res2.getTrustMeter())
                        return -1;
                    else
                        if (res1.getTrustMeter() == res2.getTrustMeter())
                            return 0;
                        else
                            return 1;
                }
        }

        /**
         * This function gets results of a task,  the results will be represented as list of Result object:
         * categoryId, rank, trustMeter.
         */
        public List<Result> getURLResults(String taskId, String url)
        {
            // 1. Instantiate the connection
            SqlConnection conn = new SqlConnection(SettingsReader.getConnectionString());
            SqlDataReader rdr = null;
            List<Result> resultsCrawled = new List<Result>();
            
            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT ResultID,Url,CategoryID,rank,TrustMeter" + 
                                " FROM Results " + 
                                "WHERE TaskID=\'" + taskId + "\' AND Url = \'" + url + "\'", conn);
                rdr = cmd.ExecuteReader();

                if (rdr.HasRows)
                {
                    while (rdr.Read())
                    {
                        int rank = Convert.ToInt32(rdr["rank"].ToString().Trim());
                        int trustMeter = Convert.ToInt32(rdr["TrustMeter"].ToString().Trim());

                        Result resultItem = new Result(rdr["ResultID"].ToString(), rdr["Url"].ToString().Trim(),
                                            rdr["CategoryID"].ToString(), rank, trustMeter);

                        resultsCrawled.Add(resultItem);

                    }
                }
                else
                {
                    resultsCrawled = null;
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Exception Caught: " + e.Message);
            }
            finally
            {
                if (rdr != null) rdr.Close();
                if (conn != null) conn.Close();
            }
            
            return resultsCrawled;
        }

        /**
         * This function returns the number of the urls which already has been crawled that belongs to the 
         * given taskId.
         */
        public ulong getTotalURLs(String taskId, String categoryId)
        {
            SqlConnection conn = new SqlConnection(SettingsReader.getConnectionString());
            SqlDataReader rdr = null;

            ulong totalUrls = 0;
            try
            {
                conn.Open();
                SqlCommand cmd = null;
                if (categoryId == null)
                {
                    cmd = new SqlCommand("SELECT COUNT(TaskID) AS TotalUrls FROM Results " +
                                        "WHERE TaskID = \'" + taskId + "\'", conn);
                }
                else
                {
                    cmd = new SqlCommand("SELECT COUNT(TaskID) AS TotalUrls FROM Results " +
                                        "WHERE TaskID = \'" + taskId + "\' AND CategoryID = \'" +
                                            categoryId + "\'", conn);
                }
                rdr = cmd.ExecuteReader();
                if (rdr.HasRows)
                {
                    rdr.Read();
                    totalUrls = Convert.ToUInt32(rdr["TotalUrls"].ToString().Trim());
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Exception Caught: " + e.Message);
            }
            finally
            {
                if (rdr != null) rdr.Close();
                if (conn != null) conn.Close();
            }

            return totalUrls;
        }

        private string getOrderString(Order order)
        {
            switch (order)
            {
                case Order.NormalOrder:
                    return "";
                case Order.AscendingRank:
                    return " ORDER BY rank ASC";
                case Order.DescendingRank:
                    return " ORDER BY rank DESC";
                case Order.AscendingTrust:
                    return " ORDER BY TrustMeter ASC";
                case Order.DescendingTrust:
                    return " ORDER BY TrustMeter DESC";
            }
            return "";
        }

         /**
          * This function returns a list of urls which suits a specific category, every url will be described 
          * as object which contains data about: url, categoryId, rank, trustMeter.
          */
         public List<Result> getURLsFromCategory(String taskId, String categoryId, Order order, int from, int to)
         {
             SqlConnection conn = new SqlConnection(SettingsReader.getConnectionString());
             SqlDataReader rdr = null;
             List<Result> resultUrls = new List<Result>();
            
             try
             {
                 conn.Open();
                 SqlCommand cmd = null;
                 string orderStr = getOrderString(order);
                 if (categoryId == null)
                 {
                     cmd = new SqlCommand("SELECT ResultID,Url,rank,TrustMeter,CategoryID From Results WHERE " +
                                            "TaskID = \'" + taskId + "\'" + orderStr, conn);
                 }
                 else
                 {
                     cmd = new SqlCommand("SELECT ResultID,Url,rank,TrustMeter,CategoryID From Results WHERE " +
                                            "TaskID = \'" + taskId + "\' AND CategoryID = \'" +
                                            categoryId + "\'" + orderStr, conn);
                 }

                 rdr = cmd.ExecuteReader();
                 if (rdr.HasRows)
                 {
                     while (rdr.Read())
                     {
                         int rank = Convert.ToInt32(rdr["rank"].ToString().Trim());
                         int trustMeter = Convert.ToInt32(rdr["TrustMeter"].ToString().Trim());
                         Result resultItem = new Result(rdr["ResultID"].ToString().Trim(), rdr["Url"].ToString().Trim(),
                                              rdr["CategoryID"].ToString().Trim(), rank, trustMeter);

                         resultUrls.Add(resultItem);

                     }
                 }
                         
                         //SqlCommand cmnd = new SqlCommand("SELECT CategoryID From Category WHERE ParentCategory = \'" +
                           //                 categoryId + "\'",conn);
                         
                        // rdr = cmnd.ExecuteReader();
                     
             }

             catch (Exception e)
             {
                 System.Console.WriteLine("Exception Caught: " + e.Message);
             }
             finally
             {
                 if (rdr != null) rdr.Close();
                 if (conn != null) conn.Close();
             }
             return rangeOfResults(resultUrls, from, to);
         }

         /**
          * This function replaces the URL result to the new one(and it's fathers).
          * NOTE:the new data stored in the data base is the data of the given newResult
          * so make sure that all the needed data there is valid(except from the resultID).
          */
         public void replaceURLResult(String taskId,Result oldResult, Result newResult)
         { 
             SqlConnection conn = new SqlConnection(SettingsReader.getConnectionString());
             
             try
             {
                 conn.Open();
                 SqlCommand cmd = new SqlCommand("DELETE FROM Results WHERE Url = \'" + oldResult.getUrl() + 
                                   "\' AND TaskID = \'" + taskId + "\'" , conn);

                 cmd.ExecuteNonQuery();

                 addURLResult(taskId, newResult);
             }
             catch (Exception e)
             {
                 System.Console.WriteLine("Exception Caught: " + e.Message);
             }
             finally
             {
                 if (conn != null) conn.Close();
             }
         }

         /**
          * This function removes one entry of the URL result, which specified in the given argument.
          * NOTE:this function removes one entry according to the resultID of the given result.
          */
         public void removeURLResult(Result result)
         {
             SqlConnection conn = new SqlConnection(SettingsReader.getConnectionString());

             try
             {
                 conn.Open();

                 SqlCommand cmd = new SqlCommand("DELETE FROM Results WHERE ResultID = \'" + result.getResultID() + "\'", conn);

                 cmd.ExecuteNonQuery();

             }
             catch (Exception e)
             {
                 System.Console.WriteLine("Exception Caught: " + e.Message);
             }
             finally
             {
                 if (conn != null) conn.Close();
             }
         }

         /**
           * This function removes all entries for specific task, which specified in the given argument.
           */
         public void removeAllResults(String taskID)
         {
             SqlConnection conn = new SqlConnection(SettingsReader.getConnectionString());

             try
             {
                 conn.Open();

                 SqlCommand cmd = new SqlCommand("DELETE FROM Results WHERE TaskID = \'" + taskID + "\'", conn);

                 cmd.ExecuteNonQuery();

             }
             catch (Exception e)
             {
                 System.Console.WriteLine("Exception Caught: " + e.Message);
             }
             finally
             {
                 if (conn != null) conn.Close();
             }
         }

         /**
          * This function adds the URL result to the given categories (and it's fathers).
          */
         public void addURLResult(String taskId, Result result)
         {
             SqlConnection conn = new SqlConnection(SettingsReader.getConnectionString());

             SqlDataReader rdr = null;

             String categoryID = result.getCategoryID();

             try
             {
                 conn.Open();

                 SqlCommand cmdcheck = new SqlCommand("SELECT TaskName FROM Task  WHERE TaskID = \'" + taskId + "\'",conn);

                 rdr = cmdcheck.ExecuteReader();
                 if (rdr.HasRows)
                 {
                     if (rdr != null) rdr.Close();
                     if ((categoryID == "") || (categoryID == "0") || (categoryID == null))
                     {
                         //System.Console.WriteLine("1> Cat ID: " + categoryID);
                         SqlCommand cmd = new SqlCommand("INSERT INTO Results (TaskID,Url,rank,TrustMeter) " +
                                                      "Values(\'" + taskId + "\',\'" + result.getUrl() + "\',\'" +
                                                      result.getRank() + "\',\'" +
                                                      result.getTrustMeter() + "\')", conn);
                         cmd.ExecuteNonQuery();
                     }
                     else
                     {
                         while ((categoryID != null) && (categoryID != ""))
                         {
                             //System.Console.WriteLine("2> Cat ID: " + categoryID);
                             SqlCommand cmd = new SqlCommand("INSERT INTO Results (TaskID,Url,CategoryID,rank,TrustMeter) " +
                                            "Values(\'" + taskId + "\',\'" + result.getUrl() + "\',\'" +
                                            categoryID + "\',\'" + result.getRank() + "\',\'" +
                                            result.getTrustMeter() + "\')", conn);

                             cmd.ExecuteNonQuery();

                             SqlCommand cmnd = new SqlCommand("SELECT ParentCategory From Category WHERE CategoryID = \'" + categoryID + "\'", conn);

                             rdr = cmnd.ExecuteReader();

                             if (rdr.HasRows)
                             {
                                 if (rdr.Read())
                                     categoryID = rdr["ParentCategory"].ToString();
                                 else
                                     categoryID = null;
                             }
                             if (rdr != null) rdr.Close();
                         }
                     }
                 }
             }
             catch (Exception e)
             {
                 System.Console.WriteLine("Exception Caught: " + e.Message);
             }
             finally
             {
                 if (rdr != null) rdr.Close();
                 if (conn != null) conn.Close();
             }
         }

        /**
         * This method is an assistant method that finds the ID of the category "WebPage".
         */
        /*
        public String getMainCategoryID()
         {
             SqlConnection conn = new SqlConnection(SettingsReader.getConnectionString());
                
             String catID = null;

             SqlDataReader rdr = null;

             try
             {
                 conn.Open();

                 SqlCommand cmd = new SqlCommand("SELECT CategoryID FROM Category WHERE CategoryName = 'WebPage'", conn);

                 rdr = cmd.ExecuteReader();

                 rdr.Read();

                 catID = rdr["CategoryID"].ToString();
             }
             catch (Exception e)
             {
                 System.Console.WriteLine("Exception Caught: " + e.Message);
             }
             finally
             {
                 if (rdr != null) rdr.Close();
                 if (conn != null) conn.Close();
             }

             return catID;
         }
        */
        /**
         * This private method receives list of Results and sorts them according to the given  Order order,
         * int from, int to .
         */
         private List<Result> rangeOfResults(List<Result> resultsList,int from,int to)
         {
             from = Math.Max(0, from);
             to = Math.Min(to, resultsList.Count - 1);
             if(resultsList.Count > to)
                return resultsList.GetRange(from, (to - from) + 1);
             return null;
         }
    }
}
