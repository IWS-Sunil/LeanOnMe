using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataAccess.Models;
using System.Collections;
using System.Web;
using GoogleMapsApi;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace LeanOnMe.API
{
    // Project      : Lean On Me(Web Services)
    // Client       : Tim J
    // Developer    : Sunil Thakur
    // Organisation : Infrawebsoft Technologies Pvt. Ltd.
    // Title        : Menu and Item Navigation API

    public class HomeController : ApiController
    {
        // Entity Call
        LeanOnMeEntities db = new LeanOnMeEntities();

        //Method to get all Menu Pages
        public IEnumerable GetHome()
        {
            var result = db.MenuPages.Where(m => m.PageLevel == "1").ToList();
            List<CommonData> ab = new List<CommonData>();
            List<CommonData> aa = new List<CommonData>();
            string path = "/Uploads/MenuPage/";
            foreach (var i in result)
            {
                aa.Add(new CommonData
                {
                    PageID = i.PageID,
                    Text = i.PageName,
                    Level = i.PageLevel,
                    Icon = path + i.Images
                });
            }
            ab.AddRange(aa);
            return ab;
        }

        public IEnumerable GetHome(string id, string user)
        {
            string x = id;
            int value;
            if (int.TryParse(x, out value))
            {
                var res = db.MenuItems.Where(m => m.PageID == value).ToList();
                List<CommonData> lst = new List<CommonData>();
                string path = "/Uploads/AdminLevel/";
                foreach (var i in res)
                {
                    lst.Add(new CommonData
                    {
                        Icon = path + i.MenuItemIcon,
                        ID = i.MenuItemID,
                        Link = i.MenuItemLink,
                        Text = i.MenuItemText,
                        PageID = i.PageID
                    });
                }
                return lst;
            }
            //do the stuff for the else condition like where we
            // get the helperlist, safeplaces etc.
            else
            {
                #region SafePlace List Details

                if (x == "MySafePlaces")
                {
                    if (int.TryParse(user, out value))
                    {
                        var res = db.MySafePlaces.Where(m => m.UserID == value).ToList();
                        List<CommonData> lst = new List<CommonData>();
                        string path = "/Uploads/MySafePlaces/";
                        foreach (var i in res)
                        {
                            lst.Add(new CommonData
                            {
                                BusinessNumber = i.BusinessNumber,
                                City = i.City,
                                Coordinates = i.Coordinates,
                                HelperID = i.HelperID,
                                HomeNumber = i.HomeNumber,
                                Mobile = i.Mobile,
                                Notes = i.Notes,
                                ID = i.PlaceID,
                                Text = i.PlaceName,
                                State = i.State,
                                StreetAddress = i.StreetAddress,
                                UserID = i.UserID,
                                ZipCode = i.ZipCode,
                                Icon = path + i.Images
                            });
                        }
                        return lst;
                    }
                }
                #endregion

                #region Getting CallerList Details
                if (x == "MyCallerList")
                {
                    if (int.TryParse(user, out value))
                    {
                        var res = db.MyCallTrees.Where(m => m.UserID == value).ToList();
                        List<CommonData> lst = new List<CommonData>();
                        string path = "/Uploads/MyCallTree/";
                        foreach (var i in res)
                        {
                            lst.Add(new CommonData
                            {
                                BusinessNumber = i.BusinessNumber,
                                City = i.City,
                                Coordinates = i.Coordinates,
                                HelperID = i.HelperID,
                                HomeNumber = i.HomeNumber,
                                Mobile = i.Mobile,
                                Notes = i.Notes,
                                ID = i.CallTreeID,
                                Text = i.Name,
                                State = i.State,
                                StreetAddress = i.StreetAddress,
                                UserID = i.UserID,
                                ZipCode = i.ZipCode,
                                Icon = path + i.Images,
                                Role = i.Role,
                                Organisation = i.Organistaion
                            });
                        }
                        return lst;
                    }
                }
                #endregion

                #region Getting Neighbor List Details
                if (x == "MyNeighborList")
                {
                    if (int.TryParse(user, out value))
                    {
                        var res = db.MyNeighbors.Where(m => m.UserID == value).ToList();
                        List<CommonData> lst = new List<CommonData>();
                        string path = "/Uploads/MyNeighbors/";
                        foreach (var i in res)
                        {
                            lst.Add(new CommonData
                            {
                                BusinessNumber = i.BusinessNumber,
                                City = i.City,
                                Coordinates = i.Coordinates,
                                HelperID = i.HelperID,
                                HomeNumber = i.HomeNumber,
                                Mobile = i.Mobile,
                                Notes = i.Notes,
                                ID = i.NeighborID,
                                Text = i.Name,
                                State = i.State,
                                StreetAddress = i.StreetAddress,
                                UserID = i.UserID,
                                ZipCode = i.ZipCode,
                                Icon = path + i.Images
                            });
                        }
                        return lst;
                    }
                }
                #endregion

                #region Doctor Lists Details
                if (x == "MyDoctorList")
                {
                    if (int.TryParse(user, out value))
                    {
                        var res = db.DoctorLists.Where(m => m.UserID == value).ToList();                
                        List<CommonData> lst = new List<CommonData>();
                        string path = "/Uploads/MyDoctors/";
                        foreach (var i in res)
                        {
                            StringBuilder sb = new StringBuilder();
                            string sp = "<font color='yellow'><b><u> " + i.Speciality+"</u></b></font>";
                            sb.Append("<br><br> &#9658  ").Append(i.DoctorName).Append("<br><br> &#9658  ").Append(i.Mobile == "" ? i.BusinessPhoneNumber : i.Mobile).Append("<br><br> &#9658  ").Append(i.StreetAddress+" " + i.City +" "+ i.State +" "+ i.ZipCode);
                            lst.Add(new CommonData
                            {
                                Icon = path + i.Images,
                                ID=i.DoctorID,
                                Speciality = sp,                                
                                speciality_detail =sb.ToString()
                            });
                        }
                        return lst;
                    }
                }
                #endregion

                #region MyMedicine Lists Details

                if (x == "MyMedicineList")
                {
                    if (int.TryParse(user, out value))
                    {
                        var res = db.MyMedicines.Where(m => m.UserID == value).ToList();
                        List<CommonData> lst = new List<CommonData>();
                        string path = "/Uploads/MyMedicines/";
                        foreach (var i in res)
                        {
                            StringBuilder sb = new StringBuilder();
                            string sp = "<font color='yellow'><b><u> " + i.MedicineName + "</u></b></font>";
                            sb.Append("<br><br> &#9658  ").Append(i.DosagePerPill).Append(" taken ").Append(i.TimesTaken).Append(" times on ").Append(i.Weekdays);
                            //sb.Append("<br><br> &#9658  ").Append(i.DosagePerPill).Append("<br><br> &#9658  ").Append(i.Mobile == "" ? i.BusinessPhoneNumber : i.Mobile).Append("<br><br> &#9658  ").Append(i.StreetAddress + " " + i.City + " " + i.State + " " + i.ZipCode);
                            lst.Add(new CommonData
                            {
                                Icon = path + i.Images,
                                ID = i.MedicineID,
                                Speciality =sp ,
                                speciality_detail = sb.ToString()
                            });
                        }
                        return lst;
                    }
                }
                #endregion

                return null;
            }

        }


        public IEnumerable GetLocations(double lat1, double lon1, double lat2, double lon2)
        {
            //double distances = GetHome(lat1, lon1, lat2, lon2, 'K');
            var origin1 = lat1 + "," + lon1;
            var destinationB = lat2 + "," + lon2;
            string ss = DistanceMatrixRequest(origin1, destinationB);

            List<CommonData> lst = new List<CommonData>();
            lst.Add(new CommonData
            {
                ZipCode = ss
            });
            return lst;
        }

        public string DistanceMatrixRequest(string source, string Destination)
        {
            try
            {

                string urlRequest = "";

                string travelMode = "Walking"; //Driving, Walking, Bicycling, Transit.
                urlRequest = @"http://maps.googleapis.com/maps/api/distancematrix/json?origins=" + source + "&destinations=" + Destination + "&mode='" + travelMode + "'&sensor=false";

                //if (keyString.ToString() != "")
                //{
                //    urlRequest += "&client=" + clientID;
                //    urlRequest = Sign(urlRequest, keyString); // request with api key and client id
                //}

                WebRequest request = WebRequest.Create(urlRequest);
                request.Method = "POST";
                string postData = "This is a test that posts this string to a Web server.";
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse response = request.GetResponse();
                dataStream = response.GetResponseStream();

                StreamReader reader = new StreamReader(dataStream);
                string resp = reader.ReadToEnd();

                reader.Close();
                dataStream.Close();
                response.Close();

                return resp;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string Sign(string url, string keyString)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();

            // converting key to bytes will throw an exception, need to replace '-' and '_' characters first.
            string usablePrivateKey = keyString.Replace("-", "+").Replace("_", "/");
            byte[] privateKeyBytes = Convert.FromBase64String(usablePrivateKey);

            Uri uri = new Uri(url);
            byte[] encodedPathAndQueryBytes = encoding.GetBytes(uri.LocalPath + uri.Query);

            // compute the hash
            HMACSHA1 algorithm = new HMACSHA1(privateKeyBytes);
            byte[] hash = algorithm.ComputeHash(encodedPathAndQueryBytes);

            // convert the bytes to string and make url-safe by replacing '+' and '/' characters
            string signature = Convert.ToBase64String(hash).Replace("+", "-").Replace("/", "_");

            // Add the signature to the existing URI.
            return uri.Scheme + "://" + uri.Host + uri.LocalPath + uri.Query + "&signature=" + signature;
        }

        public double GetHome(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            //else if (unit == 'N')
            //{
            //    dist = dist * 0.8684;
            //}
            return (dist);
        }

        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }

        // PUT: api/Home/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Home/5
        public void Delete(int id)
        {
        }



    }
}
