using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using Newtonsoft.Json;
using System.IO;
using System.ServiceModel.Activation;

namespace HuginWS
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceContract]
    public class TPSService
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "sale?okc_id={okc_id}&password={password}",
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json)]
        string sale(string okc_id, string password)
        {
            WSResult wsResult = null;
            try
            {
                /* okc_id and password should be done with authority control. */

                string salesinfo = HttpContext.Current.Request.Headers["salesinfo"].ToString();
                SalesInfo salesInfo = JsonConvert.DeserializeObject<SalesInfo>(salesinfo);
                string filePath = String.Format("{0}{1}_{2}_{3}.json", ECRDataFolder, okc_id, salesInfo.ZNo, salesInfo.DocumentNo);
                File.WriteAllText(filePath, salesinfo);

                wsResult = new WSResult() { Code = ResponseCode.SUCCESS, Content = "Successfully" };
            }
            catch (Exception ex)
            {
                wsResult = new WSResult() { Code = ResponseCode.BADREQUEST, Content = ex.Message };
            }
            string result = JsonConvert.SerializeObject(wsResult);
            return result;
        }

        [OperationContract]
        [WebInvoke(UriTemplate = "discount?okc_id={okc_id}&password={password}",
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json)]
        string discount(string okc_id, string password)
        {
            WSResult wsResult = null;
            try
            {
                /* okc_id and password should be done with authority control. */

                string salesinfo = HttpContext.Current.Request.Headers["salesinfo"].ToString();
                SalesInfo salesInfo = JsonConvert.DeserializeObject<SalesInfo>(salesinfo);
                string filePath = String.Format("{0}{1}_{2}_{3}_promo.json", ECRDataFolder, okc_id, salesInfo.ZNo, salesInfo.DocumentNo);
                File.WriteAllText(filePath, salesinfo);

                /* 
                 * If promotion applied DiscountCode has to value.
                 * If DiscountCode empty or null sended promotion is not apply.
                 */

                salesInfo.DiscountCode = "CXKU123Z";

                /* 
                 * Discount promotional information sent by the info object.
                 * If DiscountTotal sending DiscountRate have to 0.
                 * If DiscountRate sending DiscountTotal have to 0.
                 * If there isn't DiscountNotes send to empty.
                 */
                DiscountInfo discountInfo = new DiscountInfo();
                discountInfo.DiscountTotal = 1;
                discountInfo.DiscountRate = 0;
                discountInfo.DiscountNotes = new List<string>() { "3 AL 2 ODE" };

                /*
                 * If the product is desired to apply the discount salesınfo-> saleıtems [x] object should be set of the area DiscountInfo.
                 * If the product is sent to discounts should be sent subtotal discount.
                 */
                salesInfo.DiscountInfo = discountInfo;

                /*
                 * If the product is desired to apply the discount salesınfo-> saleıtems [x] object should be set of the area DiscountInfo.
                 * If the product is sent to discounts should be sent subtotal discount.
                 */

                //salesInfo.SaleItems[0].DiscountInfo = discountInfo;

                string resultContent = JsonConvert.SerializeObject(salesInfo).Replace("\"", "'");
                wsResult = new WSResult() { Code = ResponseCode.SUCCESS, Content = resultContent };
            }
            catch (Exception ex)
            {
                wsResult = new WSResult() { Code = ResponseCode.BADREQUEST, Content = ex.Message };
            }
            string result = JsonConvert.SerializeObject(wsResult);
            return result;
        }

        [OperationContract]
        [WebGet(UriTemplate = "order?okc_id={okc_id}&password={password}&order_id={order_id}",
            ResponseFormat = WebMessageFormat.Json)]
        string order(string okc_id, string password, string order_id)
        {
            WSResult wsResult = null;
            try
            {
                /* okc_id and password should be done with authority control. */

                //Sample order content
                string orderData = File.ReadAllText(TestDataFolder + "order.json");

                //Order content includes salesinfo object.
                SalesInfo salesInfo = JsonConvert.DeserializeObject<SalesInfo>(orderData);

                string resultContent = JsonConvert.SerializeObject(salesInfo).Replace("\"", "'");
                wsResult = new WSResult() { Code = ResponseCode.SUCCESS, Content = resultContent };

            }
            catch (Exception ex)
            {
                wsResult = new WSResult() { Code = ResponseCode.BADREQUEST, Content = ex.Message };
            }
            string result = JsonConvert.SerializeObject(wsResult);
            return result;
        }

        [OperationContract]
        [WebGet(UriTemplate = "products?okc_id={okc_id}&password={password}&last_update_date={last_update_date}",
            ResponseFormat = WebMessageFormat.Json)]
        string products(string okc_id, string password, string last_update_date)
        {
            WSResult wsResult = null;
            try
            {
                /* okc_id and password should be done with authority control. */

                //Sample product list
                string productData = File.ReadAllText(TestDataFolder + "products.txt");

                //string resultContent = JsonConvert.SerializeObject(salesInfo).Replace("\"", "'");
                wsResult = new WSResult() { Code = ResponseCode.SUCCESS, Content = productData };

            }
            catch (Exception ex)
            {
                wsResult = new WSResult() { Code = ResponseCode.BADREQUEST, Content = ex.Message };
            }
            string result = JsonConvert.SerializeObject(wsResult);
            return result;
        }

        #region StaticVariables
        public static string ECRDataFolder = AppDomain.CurrentDomain.BaseDirectory + "data\\";
        public static string TestDataFolder = AppDomain.CurrentDomain.BaseDirectory + "testdata\\";
        #endregion
    }
}
