using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinQToEFConsole
{
    internal class Program
    {
        
        static void Main(string[] args)
        {
            QLSACHEntities db = new QLSACHEntities();
            Console.OutputEncoding = Encoding.UTF8;

            // Các mệnh đề thông thường
            // let - biến dùng trong câu truy vấn
            var result0 = db.BOOKS.Select(book => new
            {
                name = book.NAME,
                price = book.PRICE,
                discount = book.PRICE * 0.8 // giảm giá 20%
            });

            var result1 = from book in db.BOOKS
                          let discount = book.PRICE * 0.8 
                          select new
                          {
                              name = book.NAME,
                              price = book.PRICE,
                              discount = discount
                          };

            Console.WriteLine("Danh sách sách có giảm giá 20%:");
            foreach (var item in result1)
            {
                Console.WriteLine(item.name + " - " + item.price + " - " + item.discount);
            }


            // orderby - sắp xếp
            var result2 = from book in db.BOOKS
                          orderby book.PRICE descending // giảm dần
                          select new
                          {
                              name = book.NAME,
                              price = book.PRICE
                          };

            var result3 = db.BOOKS.OrderBy(book => book.PRICE).Select(book => new // tăng dần
            {
                name = book.NAME,
                price = book.PRICE
            });

            Console.WriteLine("\n\nDanh sách sách giảm dần theo giá:");
            foreach (var item in result2)
            {
                Console.WriteLine(item.name + " - " + item.price);
            }


            // groupby - nhóm
            var result4 = from book in db.BOOKS
                          group book by book.ID_CATEGORY into groupBook
                          select new
                          {
                              category = groupBook.Key,
                              name = groupBook.FirstOrDefault().CATEGORIES.NAME,
                              count = groupBook.Count()
                          };

            var result5 = db.BOOKS.GroupBy(book => book.ID_CATEGORY).Select(groupBook => new
            {
                category = groupBook.Key,
                name = groupBook.FirstOrDefault().CATEGORIES.NAME,
                count = groupBook.Count()
            });

            Console.WriteLine("\n\nSố lượng sách theo từng loại:");
            foreach (var item in result4)
            {
                Console.WriteLine(item.category + " - " + item.name + " - " + item.count);
            }

            Console.ReadKey();
        }
    }
}
