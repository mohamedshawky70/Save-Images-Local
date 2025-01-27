                  
public class BooksController : Controller
{
	public async Task<IActionResult> Create(BookFormVM model)
	{
		if (model.Img != null)
		{
			// Delete Old image in state edit
			if (model.ImgUrl != null)
			{
				//هل موجوده علي السيرفر لأنها ممكن تتحزف بالغلط تيجي تحزفها تاني تتفقع واحده نل
				var OldImage = Path.Combine($"{RootPath}/Images/Book", model.ImgUrl);
				if (System.IO.File.Exists(OldImage)) System.IO.File.Delete(OldImage);
			}
			var Extension = Path.GetExtension(model.Img.FileName); //.jpg, .jpeg, .png
			if (!ImgMaxAllowdExtension.Contains(Extension))
			{
				ModelState.AddModelError(nameof(model.ImgUrl), "Allowed only image with extension .jpg, .jpeg, .png");
				SelectedList();
				return View();
			}
			if (model.Img.Length > ImgMaxAllowdSize)
			{
				ModelState.AddModelError(nameof(model.ImgUrl), "Allowed only image with size 2:MB");
				SelectedList();
				return View();
			}
			string RootPath = _webHostEnvironment.WebRootPath; //...wwwroot
			var ImageName = $"{Guid.NewGuid()}{Extension}";  //[random name] /3456sd23rf.png(generate GUID To be uninq in db) 
			string ImgPath = Path.Combine($"{RootPath}/Images/Book", ImageName); // الباث كله-----> wwwroot\Images\Book\rt4wfj.png
			using var stream = System.IO.File.Create(ImgPath);//حولي الباث ده لبيتس علشان اعرف استقبل فيه صوره 
			await model.Img.CopyToAsync(stream);// (هنا بكلم الاوبريتنج سيستم يبقا يفضل Async)// إستقبل فيه الصورة
			model.ImgUrl = ImageName;// لازم تاخد قيمه في الداتابيز فهنديها اسمها 

		}
		 var book = _mapper.Map<Book>(model);
		_unitOfWord.Books.Create(book);
		_unitOfWord.Commit();
		return RedirectToAction(nameof(Details), new {id=book.Id});
	}
}

