using Microsoft.AspNetCore.Mvc;
using FormsApp.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace FormsApp.Controllers;

public class HomeController : Controller
{
   

    public HomeController()
    {
       
    }

    [HttpGet]
    public IActionResult Index(string searchString, string category)//url de bir string gönderilmiş yani bir filtreleme talep edilmiş bu stringleri parametreden dönecek.
    {
        var products = Repository.Products; //repositoryden tüm ürün bilgilerini aldık.
        if(!String.IsNullOrEmpty(searchString))// arama kutusunda string ifade varsa o ifadenin olduğu prodcutları dön ve view a gönder yoksa zaten repositorydeki tüm ürünleri döner ve view a gönderiri.
        {
            ViewBag.SearchString = searchString;
            products = products.Where(p => p.Name.ToLower().Contains(searchString)).ToList();//arama cümlesini içeren productları dönecek.
        }

        //productları belirtilen şartlarda filtrelemiş olduk. p => p. notasyonu sayesinde.

        if(!String.IsNullOrEmpty(category) && category != "0")// arama kutusunda string ifade varsa o ifadenin olduğu prodcutları dön ve view a gönder yoksa zaten repositorydeki tüm ürünleri döner ve view a gönderiri.
        {
            
            products = products.Where(p => p.CategoryId == int.Parse(category)).ToList();//arama yapılırken kullanılan filtreleme cümlesinin id sine denk gelen productları dönecek.
        }

 
       //ekstra bilgilerin viewbag ile view a gönderilmesi.
        //ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name", category); //burada bir de category ekledik çünkü seçildikten sonra onun görüntülenmesini istiyoruz

        var model = new  ProductViewModel{
          Products = products,
          Categories = Repository.Categories,
          SelectedCategory = category

        };

        return View(model); //repository üzerinden tüm ürün bilgilerini controller sınıfının action metodu aracılığıyla view üzerine gönderiyoruz.
    }

    [HttpGet]
    public IActionResult Create()//create formunu viewlama işlemini yapıyor.Bunu yapmadan öncede repositoryden kategorileri çağırıp bu kategorilerle beraber dönüyor.
    {
        ViewBag.Categories = Repository.Categories;
        return View();
    }
   
     [HttpPost]
    public async Task<IActionResult> Create( Product model, IFormFile imageFile) //Bind ile sadece name ve price bilgisini postladık.[Bind("Name","Price")] //Resim dosyası ilgili metoda gönderildi. IFormFile modele de eklenebilirdi ama bu da farklı bir yöntem.
    {
         var allowedExtensions = new[] {".jpg",".jpeg",".png"};
         var extension = Path.GetExtension(imageFile.FileName);//dosyanın yolunu buluyoruz
         var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}");//dosyaya random yeni ad veriyoruz.
         var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imageFile.FileName);//ana dizine geldik ve dosyayı name özelliği ile aktarmak istediğimiz yere aktarıyoruz.

         if(imageFile != null){//verilen uzantılardan bir dosya mı değil mi ?

             if(!allowedExtensions.Contains(extension)) 
               {
                    ModelState.AddModelError("", "Geçerli bir resim seçiniz.");
               } 
         }



        if(ModelState.IsValid)//ModelState.IsValid kullanarak product entitydeki tüm validasyon kurallarının sağlandığını doğrulmaış olduk. Sağlanıyorsa view u döner sağlanmazsa zaten ön taraftan uyarı verecek.
        {  
            if(imageFile != null){
               using (var stream = new FileStream(path, FileMode.Create))
               {
               await imageFile.CopyToAsync(stream);
               }
            }
          model.Image= randomFileName;
          model.ProductId = Repository.Products.Count+1; //product ıd bilgisinide repositoryde toplam kaç adet product varsa onu ata.

          Repository.CreateProduct(model); //repositoryde ki createproduct ı çağırıp modeli gönderdik.

          return RedirectToAction("Index");// view() a döndermedik çünkü form bilgileri girildikten sonra tekrar form gelsin istemeyiz mantıken.Düşününce geri ındex sayfasının view'na dönmek mantıklı. dolayısıyla eğer başka bir action metodun view değerini dönmek istersek "RedirectToAction("dönülmek istnenen action adı")" bunu kullanırız.
        }

         ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
       

        return View(model);// önce repoya göndermiş olduğumuz modeli burada tekrar view üzerine alıp gönderdik çünkü zaten hali hazırda girmiş olduğu değerler kaybolmasın form sıfırlanarak gelmesin.
       
    }
   
    [HttpGet]
    public IActionResult Edit(int? id){ //int değerli id ifadesi gönderildi

        if(id==null){ //id gönderilmediyse boşsa not found 404 hatası dön.
            return NotFound();
        }
        var entity = Repository.Products.FirstOrDefault(p => p.ProductId == id);//id repositorydeki product id lerin biriyle eşleşiyorsa eşleşen değeri entitye ata ve view da onu dön
        if(entity == null)
        {
           return NotFound();
        }

        ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(entity);
    }
   
    [HttpPost]
    public async Task<IActionResult> Edit(Product model, int id, IFormFile imageFile){

        if(id != model.ProductId){
            return NotFound();
        }

        var extension = "";
        if(ModelState.IsValid){
              
               extension = Path.GetExtension(imageFile.FileName);//dosyanın yolunu buluyoruz
               var randomFileName = string.Format($"{Guid.NewGuid().ToString()}{extension}");//dosyaya random yeni ad veriyoruz.
               var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", imageFile.FileName);//ana dizine geldik ve dosyayı name özelliği ile aktarmak istediğimiz yere aktarıyoruz.
                
               if(imageFile != null){
               using (var stream = new FileStream(path, FileMode.Create))
               {
               await imageFile.CopyToAsync(stream);
               }

               model.Image = randomFileName;
             }

            Repository.EditProduct(model);//model bilgisini repositoryde ki editproduct içine verdik.
            return RedirectToAction("Index");


        }
      // ViewBag.Categories = new SelectList(Repository.Categories, "CategoryId", "Name");
        return View(model);
        
    }

     public IActionResult Delete(int? id)
 {
     if(id == null)
     {
         return NotFound();        
     }

     var entity = Repository.Products.FirstOrDefault(p => p.ProductId == id);
     if(entity == null)
     {
         return NotFound();
     }

     return View("DeleteConfirm", entity);
 }
 
  [HttpPost]
 public IActionResult Delete(int id, int ProductId)
 {
     if(id != ProductId)
     {
         return NotFound();
     }

     var entity = Repository.Products.FirstOrDefault(p => p.ProductId == ProductId);
     if(entity == null)
     {
         return NotFound();
     }

     Repository.DeleteProduct(entity);
     return RedirectToAction("Index");
 }
  
   [HttpPost]
 public IActionResult EditProducts(List<Product> Products)
 {
     foreach (var product in Products)
     {
         Repository.EditIsActive(product);
     }
     return RedirectToAction("Index");
 }









}

