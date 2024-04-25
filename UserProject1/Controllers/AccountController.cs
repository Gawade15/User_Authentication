using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserProject1.Models;

namespace UserProject1.Controllers
{
    public class AccountController : Controller
    {
        private ProjectDataEntities1 dbContext = new ProjectDataEntities1(); // Replace YourDbContext with your actual DbContext class    
        public ActionResult Index(int? userId)
        {

            // Check if the user is logged in or if a specific userId is provided
            if (User.Identity.IsAuthenticated || userId.HasValue)
            {
                // Retrieve user details from the database based on the logged-in user or specific userId
                int loggedInUserId = userId ?? Convert.ToInt32(User.Identity.Name);
                var user = dbContext.UserDetails.FirstOrDefault(u => u.UserId == loggedInUserId);

                if (user != null)
                {
                    if(!string.IsNullOrEmpty(user.Profile_Image))
            {
                        if (!user.Profile_Image.StartsWith("data:image/jpeg;base64,"))
                        {
                            // If Profile_Image does not start with the correct prefix, add it
                            user.Profile_Image = "data:image/jpeg;base64," + user.Profile_Image;
                        }
                    }

                    return View(user);
                }
                else
                {
                    // Handle the case where the user is not found
                    return HttpNotFound();
                }
            }
            else
            {
                // Redirect to the login page if the user is not authenticated
                return RedirectToAction("Login", "Account");
            }
        }


        // Replace YourDbContext with your actual DbContext class

        // Registration Action
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
     
        public ActionResult Register(UserDetails userModel, HttpPostedFileBase profileImageFile, bool isAdmin = false)
        {
            if (ModelState.IsValid)
            {
                // Check if mobile number is already registered
                if (dbContext.UserDetails.Any(u => u.MobileNumber == userModel.MobileNumber))
                {
                    ModelState.AddModelError("", "Mobile number is already registered.");
                    return View(userModel);

                }


                // If not registered, proceed with registration
                var newUser = new UserDetails

                {
                    UserId = userModel.UserId,
                    MobileNumber = userModel.MobileNumber,
                    UserName = userModel.UserName,
                    Email = userModel.Email,
                    Password = userModel.Password,
                    Confirm_Password = userModel.Confirm_Password,
                    Profile_Image = userModel.Profile_Image,
                    IsActive = true,
                    IsAdmin = isAdmin
                };


                // Convert the profile image to a byte array


                if (profileImageFile != null && profileImageFile.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(profileImageFile.InputStream))
                    {
                        var profileImageBytes = binaryReader.ReadBytes(profileImageFile.ContentLength);
                        newUser.Profile_Image = Convert.ToBase64String(profileImageBytes);
                    }
                }


                // Save the new user to the database
                dbContext.UserDetails.Add(newUser);
                dbContext.SaveChanges();

                // Redirect based on user role
                if (isAdmin)
                {
                    // Admin registration: Redirect to admin dashboard or any admin-specific page
                    return RedirectToAction("Index", "Admin");
                }

                else
                {
                    // User registration: Redirect to user-specific page or login
                    return RedirectToAction("Login");
                }
            }

            // If ModelState is not valid, return to the view with errors
            return View();
        }

        // Login Action
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserDetails userModel)
        {
            var user = dbContext.UserDetails.FirstOrDefault(u => u.MobileNumber == userModel.MobileNumber && u.Password == userModel.Password);

            if (user != null )
            {
                if(user.IsActive == false)
                {
                    ModelState.AddModelError("", "Your account is not activated yet.");
                    return View();
                }
                if (user.IsAdmin)
                {
                    // Admin login: Redirect to admin dashboard or any admin-specific page
                    return RedirectToAction("Index", "Admin");
                }
                // Authentication successful, you can set authentication cookie or session here
                else {
                    return RedirectToAction("Index", "Account", new { userId = user.UserId });

                }
                
                //return RedirectToAction("Index", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Invalid mobile number or password");
                return View(userModel);

            }
        }



        // Logout Action    
        public ActionResult Logout()
        {
            // Logout code here
                

            return RedirectToAction("Login");
        }

        [HttpGet]
        public ActionResult EmailLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EmailLogin(UserDetails userModel)
        {
            var user = dbContext.UserDetails.FirstOrDefault(u => u.Email == userModel.Email && u.Password == userModel.Password);

            if (user != null)
            {
                if (user.IsActive == false)
                {
                    ModelState.AddModelError("", "Your account is not activated yet.");
                    return View();
                }
                if (user.IsAdmin)
                {
                    // Admin login: Redirect to admin dashboard or any admin-specific page
                    return RedirectToAction("Index", "Admin");
                }
                // Authentication successful, you can set authentication cookie or session here
                else
                {
                    return RedirectToAction("Index", "Account", new { userId = user.UserId });

                }

                //return RedirectToAction("Index", "Account");
            }
            else
            {
                ModelState.AddModelError("", "Invalid Email or password");
                return View(userModel);

            }
        }





        public ActionResult Activate(int id)
        {
            var user = dbContext.UserDetails.Find(id);
            if (user != null)
            {
                user.IsActive = true;
                dbContext.SaveChanges();
            }

            // Redirect back to the Index page after activation
            return RedirectToAction("Index");
        }

        // Deactivate Action
        public ActionResult DeActivate(int id)
        {
            var user = dbContext.UserDetails.Find(id);
            if (user != null)
            {
                user.IsActive = false;
                dbContext.SaveChanges();
            }

            // Redirect back to the Index page after deactivation
            return RedirectToAction("Index");
        }

        // Delete Action
        public ActionResult Delete(int id)
        {
            var user = dbContext.UserDetails.Find(id);
            if (user != null)
            {
                dbContext.UserDetails.Remove(user);
                dbContext.SaveChanges();
            }

            // Redirect back to the Index page after deletion
            return RedirectToAction("Index");
        }


        //Edit Controller
        public ActionResult Edit(int id)
        {
            // Retrieve user details by ID
            var user = dbContext.UserDetails.Find(id);

            if (user == null)
            {
                return HttpNotFound(); // Or return a custom error view
            }

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserDetails userModel)
        {
            if (ModelState.IsValid)
            {
                // Update the user details in the database
                var existingUser = dbContext.UserDetails.Find(userModel.UserId);

                if (existingUser != null)
                {
                    existingUser.MobileNumber = userModel.MobileNumber;
                    existingUser.UserName = userModel.UserName;
                    existingUser.Email = userModel.Email;
                    existingUser.Password = userModel.Password;
                    existingUser.Confirm_Password = userModel.Confirm_Password;
                    existingUser.Profile_Image = userModel.Profile_Image;
                    
                    dbContext.SaveChanges();

                    return RedirectToAction("Index"); // Redirect to the user list after successful update
                }
                else
                {
                    return HttpNotFound(); // Or return a custom error view
                }
            }

            // If ModelState is not valid, return to the view with errors
            return View(userModel);




        }


        

        
    }
    }