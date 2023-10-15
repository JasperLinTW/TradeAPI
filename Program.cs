using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeAPI
{
	internal class Program
	{
		static void Main(string[] args)
		{
			//模擬傳入訂單資訊
			TradeInput input = new TradeInput()
			{
				orderId = "T01",
				storeId = "TPstore",
				amount = 100,
				encPayInfo = "{\"payMethod\":\"creditCard\",\"payToken\":\"cardnumber\"}"
			};
			//模擬解密付款資訊
			PayInfo payInfo = JsonConvert.DeserializeObject<PayInfo>(input.encPayInfo);
			StoreInfo store = new StoreInfo(input.storeId);

			Console.WriteLine($"顧客準備在{store.storeName}結帳，使用{payInfo.payMethod}付款{input.amount}元，卡號為{payInfo.payToken}");

			var tradeService = new TradeService();
			var payStr = tradeService.GeneratePayStr(input, store, payInfo, out Record record);

			Order order = new Order() { orderId = input.orderId, Record = record };
			Console.WriteLine($"交易成功，交易資訊為：\r\n{payStr}");


			//模擬傳入訂單資訊
			TradeInput refund_Input = new TradeInput()
			{
				orderId = "T02",
				ori_orderId = "T01",
				storeId = "TPstore",
				amount = 50,
			};
			//省略取得店家資訊
			StoreInfo refund_Store = store;
			//省略取得舊訂單資料
			var oriOrder = order;

			Console.WriteLine($"顧客準備在{refund_Store.storeName}退貨，原始訂單為{refund_Input.ori_orderId}，退款金額{refund_Input.amount}將由{oriOrder.Record.payMethod}退回");

			var refundStr = tradeService.GenerateRefundStr(refund_Input, refund_Store, oriOrder);
			Console.WriteLine($"退款成功，退款資訊為：\r\n{refundStr}");
		}
	}
	public class TradeService
	{
		public string GeneratePayStr(TradeInput input, StoreInfo store, PayInfo pay, out Record record)
		{
			var result = new StringBuilder();
			result.Append($"Field1：{input.amount}\r\n");
			result.Append($"Field2：{store.storeName}\r\n");
			result.Append($"Field3：{ pay.payToken}\r\n");
			result.Append($"Field4：{(store.tradeNum + 1).ToString()}\r\n");

			record = new Record()
			{
				tradeNum = store.tradeNum + 1,
				payMethod = pay.payMethod,
				payToken = pay.payToken,

			};
			return result.ToString();
		}
		public string GenerateRefundStr(TradeInput input, StoreInfo store,Order originOrder)
		{
			var result = new StringBuilder();
			result.Append($"Field1：{input.amount}\r\n");
			result.Append($"Field2：{store.storeName}\r\n");
			result.Append($"Field3：{originOrder.Record.payToken}\r\n");
			result.Append($"Field4：{originOrder.Record.tradeNum.ToString()}\r\n");

			return result.ToString();
		}

	}
	public class TradeInput
	{
		public string orderId { get; set; }
		public string ori_orderId { get; set; }
		public string storeId { get; set; }
		public int amount { get; set; }
		public string encPayInfo { get; set; }
	}
	public class StoreInfo
	{
        public StoreInfo(string storeId)
        {
            this.id = storeId;
			this.storeName = "台北店";
			this.tradeNum = 0;

		}
        public string id { get; set; }
		public string storeName { get; set; }
		public int tradeNum { get; set; }

	}
	public class PayInfo
	{
		public string payMethod { get; set; }
		public string payToken { get; set; }
	}
	public class Record
	{
		public int tradeNum { get; set; }
		public string payMethod { get; set; }
		public string payToken { get; set; }
	}
	public class Order
	{
        public string orderId { get; set; }
		public Record Record { get; set; }
 	}
}
