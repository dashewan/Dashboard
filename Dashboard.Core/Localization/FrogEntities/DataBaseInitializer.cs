using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace Dashboard.Core.FrogEntities
{
    public class DashboardContextInitializer : DropCreateDatabaseIfModelChanges<DashboardDbContext>
    {
        protected override void Seed(DashboardDbContext context)
        {
            //执行SQL语句创建数据
            //context.Database.ExecuteSqlCommand(File.ReadAllText("AddData.sql"));

            //context.Database.ExecuteSqlCommand("alter table Customer with nocheck ADD constraint index_Customer unique(CustomerName)");

            ////linqtoentity 创建数据
            //context.CustomerRank.Add(new CustomerRank() { RankName = "管理员" });
            //context.CustomerRank.Add(new CustomerRank() { RankName = "普通客户" });
            //context.CompanyInformation.Add(new CompanyInformation() { Address = "地址", CompanyName = "奔驰", CopyRights = "CopyRights", Fax = "0592-1111111", RecordNumber = "闽0001", Telphone = "0592-222", WebDescription = "描述", WebKeywords = "关键字", WebTitle = "daimlerTMS" });

            //context.SaveChanges();

            //context.Database.ExecuteSqlCommand(@"insert into Customer(CustomerName, ContactName, Sex, Email, MobilePhone, Telephone, Fax, RankID, EncryptedPassWord,IsEnterprise,CameraInfID,NeedEmailRemind,ContactPerson,IsConfirmed)
            //                                     values({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, CONVERT(varbinary(255), pwdencrypt({8})),{9},{10},{11},{12},{13})",
            //      "admin", "奔驰", 1,
            //       "56070873@qq.com", "11111", "11111", "11111", 1, "admin", 1, null, 1, "admin", 1); //customer.PassWord

            //context.Categories.Add(new Category() { CategoryId = Guid.NewGuid().ToString(), Name = "Category2" });
            //context.SaveChanges();
        }
    }
}
