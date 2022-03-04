using System;
using System.Collections.Generic;
using System.Web.Mvc;
using static AlsatPardakht_MVC.AlSatPardakht;

namespace AlsatPardakht_MVC.Controllers
{
	public class HomeController : Controller
	{
		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public ActionResult Index(string test)
		{
			// --------------- Start Define Request Value --------------
			var tahsimList = new List<Tashim>
		   {
				new Tashim { Name = "داود", Family = "میرزائی", CodeMelli = "1372566801", Shaba = "232323", Price = "1212" },
				new Tashim { Name = "حمید", Family = "موسوی", CodeMelli = "1373467781", Shaba = "23333", Price = "343"}
		   };

			MakeRequest Request = new MakeRequest
			{
				Amount = 20000,
				InvoiceNumber = "1001",
				RedirectAddress="",
				Tashims = tahsimList
			};
			// --------------- End Define Request Value --------------

			// Step 1
			ResponseHttpRequest requestResult = HttpRequestToAlSatForVerify(Request, PaymentMethod.Vaset);
			if (requestResult.IsSuccess == "1" && requestResult.Token != null && requestResult.Sign != null)
			{
				//Step2
				return Redirect(RedirectUserToPaymentPage(requestResult.Token, PaymentMethod.Vaset));
			}
			else
			{
				ViewBag.SuccessResult = "اطلاعات ارسال شده به سیستم پرداخت صحیح نمی باشد!";
				return View();
			}
		}

		[HttpGet]
		public ActionResult About(string tref, string iN, string iD)
		{
			if (tref != null && iN != null && iD != null) 
			{
				PaymentRequestResult result = new PaymentRequestResult();
				result.tref = tref;
				result.iD = iD;
				result.iN = iN;

				// Step 3
				GetPaymentInfo verifyPaymennt = VerifyPayment(tref, iN, iD, 20000, PaymentMethod.Vaset);
				if (verifyPaymennt.PSP !=null && verifyPaymennt.Verify != null)
				{
					if (verifyPaymennt.PSP.IsSuccess == true && verifyPaymennt.Verify.IsSuccess == true)
					{
						// Handle Data 
						// Example:
						//	sdsd = 	verifyPaymennt.PSP.TraceNumber;
						//	sbfd = verifyPaymennt.Verify.ShaparakRefNumber;
						// Save In DataBase
						ViewBag.Message = "Payment Is Successful";
						return View();
					}
					else
					{
						ViewBag.Message = "Payment Is Not Successful";
						return View();
					}
				}
				else
				{
					ViewBag.Message = "Payment Is Not Successful";
					return View();
				}
			}
			else
			{
				ViewBag.Message = "انصراف از پرداخت ";
				return View();
			}
		}

		public ActionResult Contact()
		{
			return View();
		}
	}
}