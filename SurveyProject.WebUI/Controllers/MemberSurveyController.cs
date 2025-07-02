using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;
using Survey.DTOs.TakeSurveyDto;
using Survey.Service.Extensions;
using SurveyProject.Business.Abstract;
using SurveyProject.Entities.Entity;
using System.Security.Claims;

namespace SurveyProject.WebUI.Controllers
{
    [Authorize]
    public class MemberSurveyController : Controller
    {
        private readonly ISurveyService _surveyService;
        private readonly IQuestionsService _questionsService;
        private readonly IAnswerOptionsService _answerOptionsService;
        private readonly IUserAnswerService _userAnswerService;
        private readonly IToastNotification _toastNotification;
        public MemberSurveyController(ISurveyService surveyService, IToastNotification toastNotification, IQuestionsService questionsService, IAnswerOptionsService answerOptionsService, IUserAnswerService userAnswerService)
        {
            _surveyService = surveyService;
            _toastNotification = toastNotification;
            _questionsService = questionsService;
            _answerOptionsService = answerOptionsService;
            _userAnswerService = userAnswerService;
        }

        //Anket soruları ve cevaplarını önizleme sayfası
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateSurveyLink(int surveyId) // surveyId'yi parametre olarak alıyoruz
        {
            var survey = _surveyService.TGetById(surveyId); // Veritabanından anketi bul
            if (survey == null)
            {
                TempData["ErrorMessage"] = "Belirtilen anket bulunamadı.";
                return NotFound();
            }

            // Host adresini dinamik olarak almak için Request.Scheme ve Request.Host kullanın.
            // Bu, uygulamanızın hangi domainde çalıştığına bağlı olarak doğru linki oluşturacaktır.
            // Buradaki "TakeSurvey" ve "Survey", anketin halka açık olarak görüntülendiği controller/action'dır.
            var surveyUrl = Url.Action("TakeSurvey", "MemberSurvey", new { id = survey.Id }, Request.Scheme, Request.Host.ToUriComponent());


            if (surveyUrl == null)
            {
                TempData["ErrorMessage"] = "Anket linki oluşturulurken bir hata oluştu.";
                return RedirectToAction("Index");
            }

            survey.Url = surveyUrl;
            _surveyService.TUpdate(survey);
            // Değişiklikleri veritabanına kaydet
            TempData["SuccessMessage"] = "Anket linki başarıyla oluşturuldu!";
            return RedirectToAction("Index", "Survey", new { area = "Admin" }); // Anket listeleme sayfasına geri dön

        }


        /// <summary>
        /// Anket linkine tıklandığında anket detaylarını ve sorularını gösteren Action.
        /// İlk ziyaret veya kaldığı yerden devam etmek için kullanılır.
        /// </summary>     
        public async Task<IActionResult> TakeSurvey(int id, int currentQuestionIndex = 0)
        {
            var survey = await _surveyService.TGetSurveyWithQuestionsAndOptionsAsync(id);

            if (survey == null || !survey.IsActive)
            {
                return View("SurveyNotFound");
            }

            var allQuestions = survey.Questions.OrderBy(q => q.Id).ToList();

            // 1. Kullanıcının ID'sini al
            var userId = User.Identity.IsAuthenticated ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value : null;

            // 2. Kullanıcının daha önce veritabanına kaydettiği cevapları getir
            // Bu metodun kullanıcının SurveyId ve UserId'sine göre cevaplarını getirmesi gerekir.
            // Eğer anonim kullanıcılar için de ilerleme kaydedilecekse, burada farklı bir mekanizma düşünülmeli (örn: unique cookie ID)
            var dbUserAnswers = new Dictionary<int, int>();
            if (userId != null)
            {
                var answersFromDb = _userAnswerService.TGetUserAnswersForSurveyAsync(id, userId);
                if (answersFromDb != null)
                {
                    foreach (var answer in answersFromDb)
                    {
                        dbUserAnswers[answer.QuestionId] = answer.SelectedOptionId;
                    }
                }
            }

            // 3. Oturumdaki cevapları da kontrol et (geçici olarak kullanılır)
            // Eğer oturumda bir cevap varsa ve bu, veritabanındaki cevaptan daha güncelse veya veritabanında yoksa onu kullan.
            var sessionUserAnswers = HttpContext.Session.GetObjectFromJson<Dictionary<int, int>>($"SurveyAnswers_{id}") ?? new Dictionary<int, int>();


            var userAnswers = new Dictionary<int, int>(dbUserAnswers);
            foreach (var sessionAnswer in sessionUserAnswers)
            {
                userAnswers[sessionAnswer.Key] = sessionAnswer.Value;
            }


            // Kullanıcının tüm soruları cevaplayıp cevaplamadığını kontrol et
            if (userAnswers.Count == allQuestions.Count && userAnswers.Count > 0)
            {
                // Kullanıcı admin ise soruları görüntülesin teşekkür sayfasına gitmesin
                if (User.IsInRole("Admin")) 
                {
                    // Admin ise son soruyu görüntülemeye devam et
                    currentQuestionIndex = Math.Max(0, allQuestions.Count - 1);
                }
                else
                {
                    // Tüm sorular cevaplanmışsa teşekkür sayfasına yönlendir ve session'ı temizle
                    HttpContext.Session.Remove($"SurveyAnswers_{id}");
                    return RedirectToAction("ThankYou", new { surveyId = id });
                }
            }

            // Kullanıcı bir soruyu atladıysa veya yeni başladıysa, en son cevaplamadığı soruya yönlendir
            // Bu döngü, `currentQuestionIndex`'i ilk cevaplanmamış soruya ayarlar.
            int startingIndex = 0; // Varsayılan olarak ilk soru
            for (int i = 0; i < allQuestions.Count; i++)
            {
                if (!userAnswers.ContainsKey(allQuestions[i].Id))
                {
                    startingIndex = i;
                    break; // İlk cevaplanmamış soruyu bulur bulmaz döngüyü kır
                }
            }
            // Eğer URL'den gelen currentQuestionIndex, kullanıcının ilerlemesinden daha ileride değilse, ilerlemesini koru.
            // Veya direkt olarak belirlenen startingIndex'i kullan.
            currentQuestionIndex = Math.Max(0, Math.Min(currentQuestionIndex, allQuestions.Count - 1));

            var currentQuestion = allQuestions.ElementAtOrDefault(currentQuestionIndex);
            if (currentQuestion == null)
            {
                return View(new ListTakeSurveyDto());
            }

            var surveyDto = new ListTakeSurveyDto
            {
                SurveyId = survey.Id,
                SurveyTitle = survey.Title,
                SurveyDescription = survey.Description,
                TotalQuestions = allQuestions.Count,
                CurrentQuestionIndex = currentQuestionIndex,
                ShowWelcomeMessage = (currentQuestionIndex == 0),
                CurrentQuestion = new QuestionDto
                {
                    Id = currentQuestion.Id,
                    Text = currentQuestion.Text,
                    AnswerOptions = currentQuestion.AnswerOptions.Select(cs => new AnswerOptionDto
                    {
                        Id = cs.Id,
                        Text = cs.Text
                    }).ToList()
                },
                UserAnswers = userAnswers // Kullanıcının mevcut cevaplarını da View'e gönder
            };

            return View(surveyDto);
        }

        /// <summary>
        /// Kullanıcının bir soruyu cevaplayıp gönderdiğinde çalışacak Action.
        /// Cevabı kaydeder ve bir sonraki soruya yönlendirir.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAnswer([FromBody] SubmitAnswerRequestDto requestDto)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Geçersiz istek verisi." });
            }

            var survey = await _surveyService.TGetSurveyWithQuestionsAndOptionsAsync(requestDto.SurveyId);
            if (survey == null || !survey.IsActive)
            {
                return Json(new { success = false, message = "Anket bulunamadı veya aktif değil." });
            }

            var question = survey.Questions.FirstOrDefault(q => q.Id == requestDto.QuestionId);
            var selectedOption = question?.AnswerOptions.FirstOrDefault(o => o.Id == requestDto.SelectedOptionId);

            if (question == null || selectedOption == null)
            {
                return Json(new { success = false, message = "Geçersiz soru veya cevap seçeneği." });
            }

            // Kullanıcının oturumdaki cevaplarını al veya yeni bir sözlük oluştur
            var userAnswers = HttpContext.Session.GetObjectFromJson<Dictionary<int, int>>($"SurveyAnswers_{requestDto.SurveyId}") ?? new Dictionary<int, int>();

            // Cevabı kaydet veya güncelle
            userAnswers[requestDto.QuestionId] = requestDto.SelectedOptionId;
            HttpContext.Session.SetObjectAsJson($"SurveyAnswers_{requestDto.SurveyId}", userAnswers);

            // Cevabı veritabanına kaydetmek için UserAnswerDto'yu kullanıyoruz
            var userAnswerDto = new UserAnswerDto
            {
                SurveyId = requestDto.SurveyId,
                QuestionId = requestDto.QuestionId,
                SelectedOptionId = requestDto.SelectedOptionId,
                UserId = User.Identity.IsAuthenticated ? User.FindFirst(ClaimTypes.NameIdentifier)?.Value : null, // ClaimTypes'ı düzgün kullanın
                SubmitDate = DateTime.Now
            };

            // UserAnswerDto'yu veritabanı modeliniz olan UserAnswer'a dönüştürün
            // Eğer service katmanınız doğrudan UserAnswerDto kabul etmiyorsa, burada dönüştürme yapmalısınız.
            // Veya ISurveyAnswerService arayüzünüzü UserAnswerDto alacak şekilde güncelleyin.
            var userAnswerModel = new UserAnswer // Models klasöründeki UserAnswer modeliniz
            {
                QuestionId = userAnswerDto.QuestionId,
                SelectedOptionId = userAnswerDto.SelectedOptionId,
                SubmitDate = userAnswerDto.SubmitDate,
                SurveyId = userAnswerDto.SurveyId,
                UserId = Guid.Parse(userAnswerDto.UserId),
            };
            _userAnswerService.TAdd(userAnswerModel); // TAdd metodu async olmalı


            // Bir sonraki soruya geçiş mantığı
            var allQuestions = survey.Questions.OrderBy(q => q.Id).ToList();
            int nextQuestionIndex = requestDto.CurrentQuestionIndex + 1;

            if (nextQuestionIndex < allQuestions.Count)
            {
                // Bir sonraki soruya yönlendir
                return Json(new { success = true, nextQuestionUrl = Url.Action("TakeSurvey", new { id = requestDto.SurveyId, currentQuestionIndex = nextQuestionIndex }) });
            }
            else
            {
                // Tüm sorular cevaplanmışsa teşekkür sayfasına yönlendir
                HttpContext.Session.Remove($"SurveyAnswers_{requestDto.SurveyId}"); // Session'ı temizle
                return Json(new { success = true, redirectUrl = Url.Action("ThankYou", new { surveyId = requestDto.SurveyId }) });
            }
        }




        public IActionResult ThankYou(int surveyId)
        {
            string surveyTitle = "Anket"; // Varsayılan başlık

            // Anket başlığını almak için servisi kullanıyoruz
            var survey = _surveyService.TGetById(surveyId);
            if (survey != null)
            {
                surveyTitle = survey.Title;
            }

            var model = new ThankYouDto
            {
                SurveyId = surveyId,
                SurveyTitle = surveyTitle
            };
            return View(model);
        }

        public IActionResult SurveyNotFound()
        {
            return View();
        }

        public IActionResult SurveyError()
        {
            return View();
        }


    }
}