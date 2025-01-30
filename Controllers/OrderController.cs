using System.Linq;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Cebelica.Models;

namespace Cebelica.Controllers
{
    public class OrderController : Controller
    {
        private readonly CebelicaDatabase _context;

        public OrderController(CebelicaDatabase context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult PlaceOrder()
        {
            ViewBag.Products = _context.Products.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult PlaceOrder(OrderModel order)
        {
            if (ModelState.IsValid)
            {
                var selectedProducts = new List<ProductsModel>();

                foreach (var entry in order.Quantities)
                {
                    if (entry.Value > 0) // Only process products with quantity > 0
                    {
                        var product = _context.Products.FirstOrDefault(p => p.Id == entry.Key);
                        if (product != null)
                        {
                            selectedProducts.Add(product);
                        }
                    }
                }

                if (selectedProducts.Any())
                {
                    // Generate order summary
                    string orderDetails = string.Join("\n", selectedProducts.Select(p =>
                        $"{order.Quantities[p.Id]}x {p.Name} (€{p.Price} each)"));

                    string emailBody = $"Order received from: {order.Email}\n\n Kontakt: {order.Contact} \n\n Naslov: {order.Address}\n\n{orderDetails}";

                    // Send email
                    SendEmail("jaka.picek@gmail.com", "New Order Received", emailBody);

                    TempData["Success"] = "Order placed successfully!";
                    return RedirectToAction("Confirmation", "Order");
                }
            }

            ViewBag.Products = _context.Products.ToList(); // Repopulate for validation errors
            return View(order);
        }

        private void SendEmail(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                //Credentials = new NetworkCredential("jaka.picek@gmail.com", "Ribncia2019!"),
                Credentials = new NetworkCredential("urban.cebelica@gmail.com", "pwkzffiotlbaichj"),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("urban.cebelica@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = false
            };

            mailMessage.To.Add(toEmail);
            smtpClient.Send(mailMessage);
        }

        public IActionResult Confirmation()
        {
            return View();
        }
    }
}
