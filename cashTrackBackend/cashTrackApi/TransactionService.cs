using cashTrackApi.Resources;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace cashTrackApi
{
  public static class TransactionService
  {
    public static async Task<int> CreateTransactionAsync(TransactionResource transaction)
    {
      var dbConnectionStr = Environment.GetEnvironmentVariable("sqldb_connection");
      using (SqlConnection conn = new SqlConnection(dbConnectionStr))
      {
        conn.Open();
        var query = $"INSERT INTO Transactions (date, description, category, account, amount) " +
          $"VALUES ({transaction.Date}, {transaction.Description}, {transaction.Category}, {transaction.Account}, {transaction.Amount})";

        using SqlCommand cmd = new SqlCommand(query, conn);
        var rows = await cmd.ExecuteNonQueryAsync();
        return rows;
      }
    }
  }
}
