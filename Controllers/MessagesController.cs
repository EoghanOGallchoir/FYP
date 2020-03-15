using FYP.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FYP.Controllers
{
    public class MessagesController : Controller
    {
        private msdb5455Entities db = new msdb5455Entities();

        // GET: Messages
        [Authorize]
        public ActionResult Home(int? Id, int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            MessageReplyViewModel vm = new MessageReplyViewModel();
            var count = db.Messages.Count();
            var user = User.Identity.Name;
            var g = db.Users.Where(x => x.UserName == user).Select(x => x.GroupId).FirstOrDefault();
            var gName = db.Groups.Where(x => x.GroupID == g).Select(x => x.GName).FirstOrDefault();
            Debug.WriteLine("Group? " + gName);
            ViewBag.g = gName;
            decimal totalPages = count / (decimal)pageSize;
            ViewBag.TotalPages = Math.Ceiling(totalPages);
            vm.Messages = db.Messages.Where(x => x.GroupID == g).OrderByDescending(x => x.DatePosted).ToPagedList(pageNumber, pageSize);
            ViewBag.MessagesInOnePage = vm.Messages;
            ViewBag.PageNumber = pageNumber;
            TempData["CurrPage"] = pageNumber;
            TempData.Keep();

            if (Id != null)
            {

                var replies = db.Replies.Where(x => x.MessageId == Id.Value).OrderByDescending(x => x.ReplyDate).ToList();
                if (replies != null)
                {
                    foreach (var rep in replies)
                    {
                        MessageReplyViewModel.MessageReply reply = new MessageReplyViewModel.MessageReply();
                        reply.MessageId = (int)rep.MessageId;
                        reply.Id = rep.Id;
                        reply.ReplyMessage = rep.ReplyMessage;
                        reply.ReplyDateTime = rep.ReplyDate;
                        reply.MessageDetails = db.Messages.Where(x => x.Id == rep.MessageId).Select(s => s.Message1).FirstOrDefault();
                        reply.ReplyFrom = rep.ReplyFromUser;
                        vm.Replies.Add(reply);
                    }

                }
                else
                {
                    vm.Replies.Add(null);
                }


                ViewBag.MessageId = Id.Value;
            }

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ReplyMessage(MessageReplyViewModel vm, int messageId)
        {
            var username = User.Identity.Name;
            
           
            if (vm.Reply.ReplyMessage != null)
            {
                Reply reply = new Reply();
                reply.ReplyDate = DateTime.Now;
                reply.MessageId = messageId;
                reply.ReplyFromUser = username;
                reply.ReplyMessage = vm.Reply.ReplyMessage;
                db.Replies.Add(reply);
                db.SaveChanges();
            }

            int pageNum = Convert.ToInt32(TempData["CurrPage"]);

            return RedirectToAction("Home", "Messages", new { Id = messageId, page = pageNum });

        }

        public ActionResult Create(int? id)
        {

            //maybe take in paramater on create view call and tempdata the groupid from that to post to group forum
            TempData["GroupID"] = id;
            TempData.Keep();
            Debug.WriteLine("G id: " + id);
            MessageReplyViewModel vm = new MessageReplyViewModel();

            return View(vm);
        }

        public ActionResult DeleteMessage(int messageID)
        {
            int id = messageID;
            return RedirectToAction("DeleteMessages", id);
        }

        [HttpPost]
        [Authorize]
        public ActionResult DeleteMessages(int messageId)
        {
            Message _messageToDelete = db.Messages.Find(messageId);
            db.Messages.Remove(_messageToDelete);
            db.SaveChanges();
            var user = User.Identity.Name;
            if (_messageToDelete.FromUser == user)
            {


                // also delete the replies related to the message
                var _repliesToDelete = db.Replies.Where(i => i.MessageId == messageId).ToList();
                if (_repliesToDelete != null)
                {
                    foreach (var rep in _repliesToDelete)
                    {
                        db.Replies.Remove(rep);
                        db.SaveChanges();
                    }
                }

            }
            return RedirectToAction("Home", "Messages");
        }

        [HttpPost]
        [Authorize]
        public ActionResult PostMessage(MessageReplyViewModel vm)
        {
            var username = User.Identity.Name;
           
            int msgid = 0;
            if (!string.IsNullOrEmpty(username))
            {
                var user = db.Users.SingleOrDefault(u => u.UserName == username);
                
            }
            int groupid = Convert.ToInt32(TempData["GroupID"]);
            Debug.WriteLine("Ggroup id? " + groupid);
            
            var g = db.Users.Where(x => x.UserName == username).Select(x => x.GroupId).FirstOrDefault();
            Debug.WriteLine("db group: "+ g);
            // var q = db.Users.Where(x => x.UserName == username).Select(x => x.GroupId);
            //var x = db.Users.Where(x => x.GroupId == q.FirstOrDefault());
            Message message = new Message();
            if (vm.Message.Subject != string.Empty && vm.Message.Message1 != string.Empty)
            {
                message.DatePosted = DateTime.Now;
                message.Subject = vm.Message.Subject;
                message.Message1 = vm.Message.Message1;
                message.FromUser = username;
                message.GroupID = (int)g;
                db.Messages.Add(message);
                db.SaveChanges();
                msgid = message.Id;
            }

            return RedirectToAction("Home", "Messages", new { Id = msgid });
        }
    }
}