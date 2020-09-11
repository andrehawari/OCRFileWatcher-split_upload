using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OCRFileWatcher.WorkerService.Config;
using OCRFileWatcher.WorkerService.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace OCRFileWatcher.WorkerService.DataAccess
{
    public class SQLAccess
    {
        private readonly ILogger<SQLAccess> _logger;
        private readonly AppSetting _appSetting;

        public SQLAccess(ILogger<SQLAccess> logger, IOptions<AppSetting> appSetting)
        {
            _logger = logger;
            _appSetting = appSetting.Value;
        }
        public List<PayerDetail> GetPayerDetails(string admissionNumber)
        {
            string sql = "SELECT distinct PayerCode, PayerName FROM dbo.PayerDetail ";
            sql += " WHERE admissionNo = @AdmissionNumber";

            SqlDataReader dr = null;
            List<PayerDetail> listPayer = new List<PayerDetail>();

            try
            {
                using (SqlConnection cnn = new SqlConnection(_appSetting.MyDB))
                {
                    // Open the connection
                    cnn.Open();

                    // Create command object
                    using (SqlCommand cmd = new SqlCommand(sql, cnn))
                    {
                        // Create parameters
                        cmd.Parameters.Add(new SqlParameter("@AdmissionNumber", admissionNumber));
                        dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        while (dr.Read())
                        {

                            PayerDetail payerDetail = new PayerDetail()
                            {
                                PayerCode = dr["PayerCode"].ToString(),
                                PayerName = dr["PayerName"].ToString()
                            };

                            listPayer.Add(payerDetail);
                        }
                    }

                }

                return listPayer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return listPayer;

        }

        public void InsertMetadata(string json)
        {
            int RowsAffected = 0;
            string ResultText;

            string sql = @"
                        INSERT INTO dbo.Metadata
                        SELECT P.DateInsert,
                        P.FileName,
                                A.AdmissionNo,
                                A.Page,
                                A.Type,
                                A.DocumentCode,
                                B.PayerCode,
                                B.PayerName,
                                A.InvoiceNumber,
                                P.Unit
                    FROM OPENJSON(@JSONData)
                            WITH(DateInsert datetime,
                                    FileName varchar(100),
                                    Unit varchar(50),
                                    DocumentTypes NVARCHAR(max) AS JSON) P
                    CROSS APPLY OPENJSON(P.DocumentTypes)
                    with
                    (
                        AdmissionNo VARCHAR(50),
                        Page int,
                        Type VARCHAR(50),
                        DocumentCode VARCHAR(50),
                        PayerDetails nvarchar(MAX) AS JSON,
                        InvoiceNumber VARCHAR(50)
                    ) A
                    OUTER APPLY OPENJSON(A.PayerDetails)
                    with
                    (
                        PayerCode VARCHAR(50),
                        PayerName VARCHAR(50)
                    ) B";

            try
            {
                using (SqlConnection cnn = new SqlConnection(_appSetting.MyDB))
                {

                    // Create command object
                    using (SqlCommand cmd = new SqlCommand(sql, cnn))
                    {
                        cmd.Parameters.Add(new SqlParameter("@JSONData", json));
                        //Set CommandType
                        cmd.CommandType = CommandType.Text;

                        // Open the connection
                        cnn.Open();

                        RowsAffected = cmd.ExecuteNonQuery();

                        ResultText = "Rows Affected:" + RowsAffected.ToString();
                    }

                }

                _logger.LogInformation(ResultText);
            }
            catch (Exception ex)
            {
                _logger.LogError("error insert json to db " + ex.Message);
            }
        }
    }
}
