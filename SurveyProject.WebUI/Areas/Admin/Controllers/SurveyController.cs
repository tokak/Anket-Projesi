using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Survey.DTOs.QuestionDto;
using Survey.DTOs.SurveyDto;
using Survey.Service.Const;
using SurveyProject.Business.Abstract;
using SurveyProject.Entities.Entity;


namespace SurveyProject.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleConsts.Admin)]
    public class SurveyController : Controller
    {
        private readonly ISurveyService _surveyService;
        private readonly IQuestionsService _questionsService;
        private readonly IAnswerOptionsService _answerOptionsService;
        private readonly IToastNotification _toastNotification;

        public SurveyController(ISurveyService surveyService, IToastNotification toastNotification, IQuestionsService questionsService, IAnswerOptionsService answerOptionsService)
        {
            _surveyService = surveyService;
            _toastNotification = toastNotification;
            _questionsService = questionsService;
            _answerOptionsService = answerOptionsService;
        }

        // Mevcut tüm anketleri listeler.
        [HttpGet]
        public IActionResult Index()
        {

            var surveys = _surveyService.TGetList().Select(x => new ListSurveyDto()
            {
                Id = x.Id,
                Description = x.Description,
                IsActive = x.IsActive,
                Title = x.Title,
                Url = x.Url,
                CreateDate = x.CreatedDate
            });
            return View(surveys);
        }


        // Yeni anket oluşturma formunu görüntüler.       
        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateSurveyDto();
            return View(model);
        }

       
        /// Yeni anket oluşturma formunun gönderimini işler.     
        [HttpPost]
        [ValidateAntiForgeryToken] 
        public IActionResult Create(CreateSurveyDto model)
        {
            if (ModelState.IsValid)
            {
                var newSurvey = new TSurvey
                {
                    Title = model.Title,
                    Description = model.Description,
                    IsActive = model.IsActive,
                    CreatedDate = DateTime.Now
                };
                _surveyService.TAdd(newSurvey);
                _toastNotification.AddSuccessToastMessage("Anket ekleme işleminiz başarılı", new ToastrOptions { Title = "Başarılı" });
                return RedirectToAction("Index", "Survey");
            }
            // Model geçerli değilse, formu doğrulama hatalarıyla birlikte tekrar göster.
            _toastNotification.AddErrorToastMessage("Lütfen formu doğru şekilde doldurunuz.", new ToastrOptions { Title = "Hata" });
            return View(model);
        }

       
      
        // Mevcut bir anketi güncellemek için formu görüntüler.
        [HttpGet]
        public IActionResult Update(int id)
        {
            var survey = _surveyService.TGetById(id);
            if (survey == null)
            {
                _toastNotification.AddErrorToastMessage("Güncellemek istediğiniz anket bulunamadı.", new ToastrOptions { Title = "Hata" });
                return RedirectToAction("Index", "Survey", new { area = "Admin" });
            }
            UpdateSurveyDto updateSurveyDto = new()
            {
                Id = survey.Id,
                Description = survey.Description,
                IsActive = survey.IsActive,
                Title = survey.Title,
                Url = survey.Url
            };
            return View(updateSurveyDto);
        }

        /// Mevcut bir anketin güncelleme formunun gönderimini işler.
        [HttpPost]
        public IActionResult Update(UpdateSurveyDto updateSurveyDto)
        {
            if (!ModelState.IsValid)
            {
                _toastNotification.AddErrorToastMessage("Lütfen formu doğru şekilde doldurunuz.", new ToastrOptions { Title = "Hata" });
                return View(updateSurveyDto);
            }

            // Güncellenecek mevcut anketi veritabanından çek.
            var existingSurvey = _surveyService.TGetById(updateSurveyDto.Id);
            if (existingSurvey == null)
            {
                _toastNotification.AddErrorToastMessage("Güncellenecek anket bulunamadı.", new ToastrOptions { Title = "Hata" });
                return RedirectToAction("Index", "Survey", new { area = "Admin" });
            }

            existingSurvey.Description = updateSurveyDto.Description;
            existingSurvey.IsActive = updateSurveyDto.IsActive;
            existingSurvey.Title = updateSurveyDto.Title;
            existingSurvey.Url = updateSurveyDto.Url;
            _surveyService.TUpdate(existingSurvey);
            _toastNotification.AddSuccessToastMessage("Güncelleme işleminiz başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction("Index", "Survey", new { area = "Admin" });
        }


        /// Belirtilen anket kimliğine sahip anketi siler.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var survey = _surveyService.TGetById(id);
            if (survey == null)
            {
                _toastNotification.AddWarningToastMessage($"Silinecek anket bulunamadı", new ToastrOptions { Title = "Uyarı" });
                return RedirectToAction("Index", "Survey", new { area = "Admin" });
            }
            _surveyService.TDelete(survey);
            _surveyService.TDeleteUserSurvey(id);
            _toastNotification.AddSuccessToastMessage("Silme işleminiz başarılı", new ToastrOptions { Title = "Başarılı" });
            return RedirectToAction("Index", "Survey", new { area = "Admin" });
        }

        public IActionResult SurveyQuestions(int surveyId, string surveyTitle)
        {
            
            var survey = _surveyService.TGetById(surveyId); 
            if (survey == null)
            {
                _toastNotification.AddErrorToastMessage("Anket bulunamadı.", new ToastrOptions { Title = "Hata" });
                return RedirectToAction("CreateSurveyQuestion", "Survey", new { surveyId = surveyId, surveyTitle = surveyTitle }); 
            }

            var model = new ListSurveyQuestionsDto
            {
                SurveyId = surveyId,
                SurveyTitle = survey.Title,
                 SurveyDesription = survey.Description
            };

            // İlgili ankete ait tüm soruları veritabanından çek
            var questions = _questionsService.TGetListByFilter(q => q.SurveyId == surveyId);

            // Her soru için cevap seçeneklerini çek ve dto'e ekle
            foreach (var question in questions)
            {
                var questionDto = new QuestionDto
                {
                    Id = question.Id,
                    Text = question.Text
                };

                var answerOptions = _answerOptionsService.TGetListByFilter(ao => ao.QuestionId == question.Id);
                foreach (var option in answerOptions)
                {
                    questionDto.AnswerOptions.Add(new AnswerOptionDto
                    {
                        Id = option.Id,
                        Text = option.Text
                    });
                }
                model.Questions.Add(questionDto);
            }
            return View(model);
        }


        // Anket sorularını oluşturma ve yönetme sayfasını görüntüler.
        // Belirtilen ankete ait mevcut soruları ve cevap seçeneklerini yükler.        
        [HttpGet]
        public async Task<IActionResult> CreateSurveyQuestion(int surveyId, string surveyTitle)
        {
            CreateSurveyQuestionDto createSurveyQuestionDto = new()
            {
                SurveyId = surveyId,
                SurveyTitle = surveyTitle,
                Questions = new List<QuestionInputDto>()
            };

            try
            {
                var survey = _surveyService.TGetById(surveyId);
                if (survey == null)
                {
                    _toastNotification.AddErrorToastMessage("Anket bulunamadı.", new ToastrOptions { Title = "Hata" });
                    return NotFound($"Anket bulunamadı.");
                }

                // Ankete ait mevcut soruları veritabanından çek
                var existingQuestions = _questionsService.TGetListByFilter(x => x.SurveyId == surveyId);

                // Her bir sorunun cevaplarını da çekerek DTO'ya dönüştür
                foreach (var q in existingQuestions)
                {
                    var questionInputDto = new QuestionInputDto
                    {
                        Id = q.Id,
                        QuestionText = q.Text,
                        AnswerOptions = new List<AnswerOptionInputDto>()
                    };

                    // Her soruya ait cevap seçeneklerini çek
                    var existingOptions = _answerOptionsService.TGetListByFilter(x => x.QuestionId == q.Id);

                    foreach (var ao in existingOptions)
                    {
                        questionInputDto.AnswerOptions.Add(new AnswerOptionInputDto
                        {
                            Id = ao.Id,
                            OptionText = ao.Text
                        });
                    }
                    createSurveyQuestionDto.Questions.Add(questionInputDto);
                }
            }
            catch (Exception ex)
            {
                _toastNotification.AddErrorToastMessage("Mevcut sorular yüklenirken bir sorun oluştu.", new ToastrOptions { Title = "Hata" });
                ViewData["LoadErrorMessage"] = "Mevcut sorular yüklenirken bir sorun oluştu: " + ex.Message; 
            }
            return View(createSurveyQuestionDto);
        }


        //Anket sorularını ve cevap seçeneklerini ekleme ve güncelleme işlemlerini işler.
        // Bu metot, tek sayfalı düzenleyiciden gelen tüm form gönderimini işler.
      
        [HttpPost]
        public IActionResult SaveSurveyQuestions(CreateSurveyQuestionDto request)
        {
            var errors = new List<string>();
            bool hasChanges = false; // Değişiklik olup olmadığını takip etmek için kullanılır

            // 1. ModelState doğrulama
            if (!ModelState.IsValid)
            {
                errors.AddRange(ModelState.Values
                                       .SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage));
            }

            // 2. İş Kuralı Doğrulama
            if (request.Questions == null || !request.Questions.Any())
            {
                // Eğer hiç soru yoksa ve daha önce de yok idiyse bu bir değişiklik değildir.
                // Ancak kullanıcı tüm soruları silip boş bir gönderim yaparsa, bu bir değişikliktir.
                // Bu senaryoyu ele almak için daha sonra veritabanındaki soruları da kontrol edebiliriz.
                // Şimdilik, boş gönderimi hata olarak kabul edelim.
                errors.Add("Kaydedilecek en az bir soru olmalıdır.");
            }
            else
            {
                foreach (var questionDto in request.Questions)
                {
                    // Tüm sorular için doğrulama yap (yeni ve mevcut)
                    if (string.IsNullOrWhiteSpace(questionDto.QuestionText))
                    {
                        errors.Add($"Bir sorunun metni boş olamaz.");
                    }

                    if (questionDto.AnswerOptions == null || questionDto.AnswerOptions.Count < 2)
                    {
                        errors.Add($"'{questionDto.QuestionText}' sorusu için en az 2 cevap seçeneği girilmelidir.");
                    }
                    else
                    {
                        if (questionDto.AnswerOptions.Any(ao => string.IsNullOrWhiteSpace(ao.OptionText)))
                        {
                            errors.Add($"'{questionDto.QuestionText}' sorusunun tüm cevap seçenekleri doldurulmalıdır.");
                        }
                    }
                }
            }

            if (errors.Any())
            {
                _toastNotification.AddErrorToastMessage("Validasyon hataları oluştu. Lütfen formu kontrol edin.", new ToastrOptions { Title = "Hata" });
                return BadRequest(new { success = false, message = "Validasyon hataları oluştu.", errors = errors });
            }

            try
            {
                // Mevcut soruları ve cevapları veritabanından çek.
                var existingQuestionsInDb = _questionsService.TGetListByFilter(x => x.SurveyId == request.SurveyId);
                var existingAnswerOptionsInDb = _answerOptionsService.TGetListByFilter(x => existingQuestionsInDb.Select(q => q.Id).Contains(x.QuestionId));

                // Formdan gelen ID'leri bir listeye al
                var sentQuestionIds = request.Questions.Where(q => q.Id != 0).Select(q => q.Id).ToList();
                var sentAnswerOptionIds = request.Questions.SelectMany(q => q.AnswerOptions.Where(ao => ao.Id != 0).Select(ao => ao.Id)).ToList();

                // 1. Silinen soruları tespit et ve sil
                var questionsToDelete = existingQuestionsInDb.Where(q => !sentQuestionIds.Contains(q.Id)).ToList();
                foreach (var qToDelete in questionsToDelete)
                {
                    _questionsService.TDelete(qToDelete);
                    hasChanges = true;
                }

                // 2. Soruları ve cevapları ekle/güncelle
                foreach (var questionDto in request.Questions)
                {
                    if (questionDto.Id == 0) // Yeni soru
                    {
                        var newQuestion = new Question
                        {
                            SurveyId = request.SurveyId,
                            Text = questionDto.QuestionText,
                        };
                        _questionsService.TAdd(newQuestion); // Yeni soruyu ekle
                        hasChanges = true;

                        foreach (var optionDto in questionDto.AnswerOptions)
                        {
                            var newOption = new AnswerOption
                            {
                                QuestionId = newQuestion.Id, // Yeni eklenen sorunun ID'sini kullan
                                Text = optionDto.OptionText
                            };
                            _answerOptionsService.TAdd(newOption);
                            hasChanges = true;
                        }
                    }
                    else // Mevcut soru - Güncelleme
                    {
                        var existingQuestion = existingQuestionsInDb.FirstOrDefault(q => q.Id == questionDto.Id);
                        if (existingQuestion != null)
                        {
                            if (existingQuestion.Text != questionDto.QuestionText)
                            {
                                existingQuestion.Text = questionDto.QuestionText;
                                _questionsService.TUpdate(existingQuestion);
                                hasChanges = true;
                            }

                            // Mevcut sorunun mevcut cevap seçeneklerini al
                            var currentOptionsForQuestion = existingAnswerOptionsInDb.Where(ao => ao.QuestionId == existingQuestion.Id).ToList();
                            var sentOptionIdsForQuestion = questionDto.AnswerOptions.Where(ao => ao.Id != 0).Select(ao => ao.Id).ToList();

                            // Silinen cevap seçeneklerini tespit et ve sil (bu soruya ait olanlar)
                            var optionsToDelete = currentOptionsForQuestion.Where(ao => !sentOptionIdsForQuestion.Contains(ao.Id)).ToList();
                            foreach (var oToDelete in optionsToDelete)
                            {
                                _answerOptionsService.TDelete(oToDelete);
                                hasChanges = true;
                            }

                            // Cevap seçeneklerini ekle/güncelle
                            foreach (var optionDto in questionDto.AnswerOptions)
                            {
                                if (optionDto.Id == 0) // Mevcut bir soruya yeni cevap seçeneği ekleme
                                {
                                    var newOption = new AnswerOption
                                    {
                                        QuestionId = existingQuestion.Id,
                                        Text = optionDto.OptionText
                                    };
                                    _answerOptionsService.TAdd(newOption);
                                    hasChanges = true;
                                }
                                else // Mevcut cevap seçeneği - Güncelleme
                                {
                                    var existingOption = currentOptionsForQuestion.FirstOrDefault(ao => ao.Id == optionDto.Id);
                                    if (existingOption != null && existingOption.Text != optionDto.OptionText)
                                    {
                                        existingOption.Text = optionDto.OptionText;
                                        _answerOptionsService.TUpdate(existingOption);
                                        hasChanges = true;
                                    }
                                }
                            }
                        }
                    }
                }

                // İşlem sonunda değişiklik olup olmadığını kontrol et
                if (hasChanges)
                {
                    _toastNotification.AddSuccessToastMessage("Anket soruları başarıyla kaydedildi!", new ToastrOptions { Title = "Başarılı" });
                    return RedirectToAction("SurveyQuestions", new { surveyId = request.SurveyId, surveyTitle = request.SurveyTitle });
                }
                else
                {
                    _toastNotification.AddInfoToastMessage("Anket sorularında herhangi bir değişiklik yapılmadı.", new ToastrOptions { Title = "Bilgi" });
                    return RedirectToAction("CreateSurveyQuestion", new { surveyId = request.SurveyId, surveyTitle = request.SurveyTitle });

                }
            }
            catch (Exception ex)
            {
                // Hata mesajını loglayın ve istemciye döndürün
                _toastNotification.AddErrorToastMessage("Anket soruları kaydedilirken bir hata oluştu.", new ToastrOptions { Title = "Hata" });
                return StatusCode(500, new { success = false, message = "Anket soruları kaydedilirken sunucu tarafında beklenmedik bir hata oluştu: " + ex.Message });
            }
        }

        // veritabanından soruyu siler.
        [HttpPost]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = _questionsService.TGetById(id);
            if (question == null)
            {
                _toastNotification.AddWarningToastMessage("Silinecek soru bulunamadı.", new ToastrOptions { Title = "Uyarı" });
                return NotFound(new { message = "Soru bulunamadı." });
            }

            try
            {
                _questionsService.TDelete(question);
                _toastNotification.AddSuccessToastMessage("Soru başarıyla silindi.", new ToastrOptions { Title = "Başarılı" });
                return Ok(new { message = "Soru başarıyla silindi." });
            }
            catch (Exception ex)
            {
                _toastNotification.AddErrorToastMessage($"Soru silinirken bir hata oluştu: {ex.Message}", new ToastrOptions { Title = "Hata" });
                return StatusCode(500, new { message = $"Soru silinirken bir hata oluştu: {ex.Message}" });
            }
        }


        // cevap seçeneğini siler.
        [HttpPost]
        public IActionResult DeleteAnswerOption(int id)
        {
            try
            {
                var answerOption = _answerOptionsService.TGetById(id);

                if (answerOption == null)
                {
                    _toastNotification.AddWarningToastMessage("Silinecek cevap seçeneği bulunamadı.", new ToastrOptions { Title = "Uyarı" });
                    return NotFound(new { success = false, message = "Cevap seçeneği bulunamadı." });
                }
                _answerOptionsService.TDelete(answerOption);
                
                _toastNotification.AddSuccessToastMessage("Cevap seçeneği başarıyla silindi.", new ToastrOptions { Title = "Başarılı" });
                return Ok(new { success = true, message = "Cevap seçeneği başarıyla silindi." });
            }
            catch (Exception ex)
            {
                _toastNotification.AddErrorToastMessage($"Cevap seçeneği silinirken bir hata oluştu: {ex.Message}", new ToastrOptions { Title = "Hata" });
                return StatusCode(500, new { success = false, message = "Cevap seçeneği silinirken bir hata oluştu: " + ex.Message });
            }
        }


        


    }
}