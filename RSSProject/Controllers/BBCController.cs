using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using System.Xml.Linq;
using RSSProject.Models;

namespace RSSProject.Controllers
{
    public class BBCController : Controller
    {
        // GET: BBC
        public ActionResult Index()
        {
            String title = null;
            List<Feed> feeds= GetRSSFeed("http://feeds.bbci.co.uk/news/rss.xml");
            
            //Create a new list with the top 10 stories from the RSS feed, ordered by date
            TopTen TopTenList = new TopTen();
            TopTenList.TopFeeds = feeds.OrderByDescending(x => x.Date).Take(10).ToList();
            SendEmail("jimthebenevolent@gmail.com", TopTenList);
            return View(TopTenList);
        }

        //Reusable class that reads XML from a provided URL
        public List<Feed> GetRSSFeed(String URL)
        {
            //Create a list to save the collection of Feed objects
            List<Feed> feeds = new List<Feed>();
            XDocument xDoc = new XDocument();
            xDoc = XDocument.Load(URL);

            //Iterate through each 'item' in the RSS feed, creating a new Feed object for each
            foreach (XElement x in xDoc.Descendants("item"))
            {
                Feed feed = new Feed();
                feed.Title = x.Element("title").Value;
                feed.Description = x.Element("description").Value;
                feed.Link = x.Element("link").Value;

                //Get the date in a format more usable (without the prefixed day)
                feed.Date = x.Element("pubDate").Value.Substring(5);
                feed.ImageURL = x.Element(x.GetNamespaceOfPrefix("media") + "thumbnail").Attribute("url").Value;
                feeds.Add(feed);
            }
            return feeds;
        }

        public void SendEmail(String address, TopTen TopTenList)
        {
            //Create a new SMTP client using Microsofts Outlook SMTP server
            SmtpClient client = new SmtpClient("smtp-mail.outlook.com");
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            //Log in using Outlook Credentials
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential("jacob.rawlinson@hotmail.co.uk", "********");
            client.EnableSsl = true;
            client.Credentials = credentials;

            //Create a HTML message containing the top ten results from the RSS feed
            String msg = "<div style='padding: 2% 10%'>";
            for (int i = 0; i < TopTenList.TopFeeds.Count(); ++i)
            {
                msg = msg + "<div style='width: 70%; display: inline-block'><h1 style='font-style: italic'>" +
                      TopTenList.TopFeeds[i].Title + "</h1><p>" + TopTenList.TopFeeds[i].Description +
                      "</p><br /><p style='color:#808080'>" + TopTenList.TopFeeds[i].Date + "</p><a href='" +
                      TopTenList.TopFeeds[i].Link + "'>" + TopTenList.TopFeeds[i].Link +
                      "</a><br /></div><div style='width: 20%; display: inline-block'><img src='" +
                      TopTenList.TopFeeds[i].ImageURL + "' style='vertical-align: middle; width: 100%'/></div><hr />";
            }
            msg = msg + "</div>";

            //Create and send the message
            MailMessage message = null;
            try
            {
                message = new MailMessage("jacob.rawlinson@hotmail.co.uk",
                    address, "Top 10 BBC Stories",
                    msg);

                message.IsBodyHtml = true;
                client.Send(message);
            }
            catch (Exception ex)
            {
                //Catches the failed log in attempt (and any other errors)
            }
        }
    }
}