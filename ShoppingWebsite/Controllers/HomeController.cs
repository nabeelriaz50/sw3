using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShoppingWebsite.Models;
using System.Data.Entity;
using System.Data;

namespace ShoppingWebsite.Controllers
{
    public class HomeController : Controller
    {
        DatabaseEntities1 db = new DatabaseEntities1();
        public ActionResult CheckCart ()
        {
            return View((List<Product>)Session["Products"]);
        }
        public ActionResult  CheckOut ()
        {
            if((Session["isUserLoggedIn"] == null))
            {
                return RedirectToAction("Login");
            }

            return View((List<Product>)Session["Products"]);

        }
        public ActionResult AddToCart()
        {
            if(Session["Products"] == null)
            {
                List <Product> products = new List<Product>();
                products.Add(db.Products.Find(int.Parse(Request["ProductId"])));
                Session["Products"] = products;
                
            }
            else
            {
                List <Product> products = (List<Product>)Session["Products"];
                products.Add(db.Products.Find(int.Parse(Request["ProductId"])));
                Session["Products"] = products;
                
            }
            return RedirectToAction("Index");
        }
        public ActionResult UserSearch ()
        {
            string name = Request["Query"];
            var products = db.Products.Where(x => x.Name.Contains(name));
            return View(products.ToList()); 
        }

        public ActionResult DeleteItem ()
        {
            int id = int.Parse(Request["ProductId"]);
            db.Products.Remove(db.Products.Find(id));
            db.SaveChanges();
            return View();   
        }

        public ActionResult AdminSearch()
        {
            string name = Request["Query"];
            var products = db.Products.Where(x => x.Name.Contains(name));
            return View(products.ToList()); 
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Welcome";
            var products = db.Products.Where(x=> x.Id >= 1);
            return View(products.ToList());
        }
        public ActionResult Login()
        {
            ViewBag.Title = "Login";
            return View();
        }

        public ActionResult ForgotPassword()
        {
            ViewBag.Title = "Forgot Password";
            return View();
        }

        public ActionResult ViewProduct ()
        {
            int id = Int32.Parse(Request["ProductId"]);
            Product p = db.Products.Find(id);
            return View(p);
        }

        [HttpPost]
        public ActionResult SecretQuestion(User user)
        {
            ViewBag.Title = "Secret Question";
            try
            {
                var exists = db.Users.First(x => x.Email.Equals(user.Email));
                user = exists;
                return View(user);
            }
            catch(Exception e)
            {
                user = null;
                return View(user);
            }

        }


        [HttpPost]
        public ActionResult CheckAnswer()
        {
            ViewBag.Title = "Checking Your Answer";
            int userId = int.Parse(Request["UserId"]);
            string answer = Request["Answer"];
            User user;
            try
            {
                var exists = db.Users.First(x => x.UserId.Equals(userId) && x.AnswerOfSecretQuestion.Equals(answer));
                user = exists;
                return View(user);
            }
            catch (Exception e)
            {
                user = null;
                return View(user);
            }

        }

        public JsonResult CheckUsernameAvailibility()
        {
            string username = Request["Username"];
            var isUsernameAvailable = db.Users.Any(x => x.Email.Equals(username));
            return this.Json(isUsernameAvailable, JsonRequestBehavior.AllowGet);

        }


        public ActionResult Logout()
        {
            if (Session["isAdminLoggedIn"] != null)
            {
                Session["isAdminLoggedIn"] = null;
            }
            else if (Session["isUserLoggedIn"] != null)
            {
                Session["isUserLoggedIn"] = null;
            }

            return RedirectToAction("Index");
        }
        
        public ActionResult AddItem()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddItem(Product product)
        {
            if (ModelState.IsValid)
            {
                HttpPostedFileBase file = Request.Files[0];

                ViewData["Error"] = "Please upload an image as your profile pic. ";
                if (!file.FileName.ToLower().Contains(".jpg") && !file.FileName.ToLower().Contains(".png")
                    && !file.FileName.ToLower().Contains(".jpeg"))
                {
                    ViewBag.Title = "Add item";
                    ViewData["Error"] = "Please upload an image of product. ";
                    return View(product);
                }
                else
                {
                    file.SaveAs(Server.MapPath(@"~\Files\" + file.FileName));
                    product.Picture= file.FileName;
                    db.Products.Add(product);
                    db.SaveChanges();
                    ViewBag.Title = "Welcome";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                return View(product);
            }
        }

        public ActionResult UpdatePassword()
        {
            ViewBag.Title = "Update Password";

            int userId = int.Parse(Request["UserId"]);
            User user = db.Users.Find(userId);
            user.Password = Request["Password"];
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index");
           
        }
        [HttpPost]
        public ActionResult Login(User User)
        {
            ViewBag.Title = "Login";

            if (User.Email.Equals("admin") && User.Password.Equals("123"))
            {
                Session["isAdminLoggedIn"] = true;
                return RedirectToAction("Index");
            }

            var exists = db.Users.Any(x => x.Email.Equals(User.Email) && x.Password.Equals(User.Password));

            if (exists)
            {
                Session["isUserLoggedIn"] = true;

                return RedirectToAction("Index");
            }
            
           return View(User);
           
        }
        public ActionResult Register()
        {
            ViewBag.Title = "Register";
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                HttpPostedFileBase file = Request.Files[0];

                ViewData["Error"] = "Please upload an image as your profile pic. ";
                if (!file.FileName.ToLower().Contains(".jpg") && !file.FileName.ToLower().Contains(".png")
                    && !file.FileName.ToLower().Contains(".jpeg"))
                {
                    ViewBag.Title = "Register";
                    ViewData["Error"] = "Please upload an image as your profile pic. ";
                    return View(user);
                }
                else
                {
                    file.SaveAs(Server.MapPath(@"~\Files\" + file.FileName));
                    user.ProfilePic = file.FileName;
                    db.Users.Add(user);
                    db.SaveChanges();
                    ViewBag.Title = "Welcome";
                    return RedirectToAction("Index");
                }
            }

            return View(user);
        }
    }
}
