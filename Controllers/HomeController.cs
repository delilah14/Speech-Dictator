using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using iTextSharp;
using iTextSharp.text;
using System.Diagnostics;
using iTextSharp.text.pdf;

using System.IO;
using VoiceDictation;

using VoiceDictation.ViewModels;
using Microsoft.CognitiveServices.Speech.Translation;

namespace VoiceDictation.Controllers
{
    public class HomeController : Controller
    {
        public static string x;
        SpeechConfig config;
        static string translateKey;
        static string translatedWords;
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> RecognizeSpeechAsync()
        {
            
           
            try
            {
                config = SpeechConfig.FromSubscription("ae9492aae8044a4c888a45a45e957d83", "westus");
               
                using (var recognizer = new SpeechRecognizer(config))
                {


                  
                    var result = await recognizer.RecognizeOnceAsync();
                    

                  
                    if (result.Reason == ResultReason.RecognizedSpeech)
                    {
                        translatedWords = translatedWords +  result.Text;
                        ViewBag.message = result.Text;
                    }
                    
                    else if (result.Reason == ResultReason.NoMatch)
                    {
                        ViewBag.message = "Not recognized";

                    }
                    else if (result.Reason == ResultReason.Canceled)
                    {
                        var cancellation = CancellationDetails.FromResult(result);
                       

                        if (cancellation.Reason == CancellationReason.Error)
                        {
                            ViewBag.message = result.Text;

                        }
                    }
                    
                }
            }

            catch (Exception ex)
            {
                x = ex.StackTrace;
            }


            return View("Index");

        }
        public async Task<ActionResult> translateAsync() {
            string fromLanguage = "en-US";


            const string GermanVoice = "Microsoft Server Speech Text to Speech Voice (de-DE, Hedda)";

            var config = SpeechTranslationConfig.FromSubscription("ae9492aae8044a4c888a45a45e957d83", "westus");
            config.SpeechRecognitionLanguage = fromLanguage;
            config.VoiceName = GermanVoice;


            config.AddTargetLanguage("de");


            using (var recognizer = new TranslationRecognizer(config))
            {

                recognizer.Recognizing += (s, e) =>
                {
                   
                    foreach (var element in e.Result.Translations)
                    {
                        ViewBag.message = element.Value;

                    }
                };

                recognizer.Recognized += (s, e) =>
                {
                    if (e.Result.Reason == ResultReason.TranslatedSpeech)
                    {
                       
                        foreach (var element in e.Result.Translations)
                        {
                            ViewBag.message = element.Value;
                        }
                    }
                    else if (e.Result.Reason == ResultReason.RecognizedSpeech)
                    {
                        foreach (var element in e.Result.Translations)
                        {
                            ViewBag.message = element.Value;
                        }
                    }
                    else if (e.Result.Reason == ResultReason.NoMatch)
                    {
                        
                    }
                };


                await recognizer.RecognizeOnceAsync();
                
                return View("Index");
            }
        }
        public void speak() {
           
        }
        public ActionResult print() {
            
            try
            {
               
                var pdfDoc = new Document(PageSize.A4,40f,40f,60f,60f);
                string path = $"C:\\Users\\Delilah Dessalegn\\Documents\\dictate.pdf";
                PdfWriter.GetInstance(pdfDoc, new FileStream(path,FileMode.Create));
                pdfDoc.Open();

                pdfDoc.Add(new Paragraph(translatedWords));
                pdfDoc.Close();

            }
            catch (Exception ex)
            {
                x = ex.Message;
            }
            return View("Index");

        }
        public ActionResult Bold() {
            
            NotepadViewModel notepadViewModel = new NotepadViewModel();
            notepadViewModel.boolCommand = "font-weight:bold";
            return View("Index",notepadViewModel);
        }
        public ActionResult clear() {
            ViewBag.message = "";
            return View("Index");
        }
        

        public ActionResult Underline() {
            NotepadViewModel notepadViewModel = new NotepadViewModel();
            notepadViewModel.underlineCommand = "text-decoration:underline";
            ViewBag.x = notepadViewModel.underlineCommand;
            return View("Index");
        }

     

    }
}