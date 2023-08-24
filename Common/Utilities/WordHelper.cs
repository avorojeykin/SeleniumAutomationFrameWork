using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Common.Utilities
{
    public static class WordHelper
    {
        public static void CreateFileWithText(string fileFullName, string textToInsert, bool openFile)
        {
            using (WordprocessingDocument wordprocessingDocument =
                WordprocessingDocument.Create(fileFullName, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
            {
                // add a main document part.
                MainDocumentPart mainPart = wordprocessingDocument.AddMainDocumentPart();

                // create the document structure and add text
                mainPart.Document = new Document();
                Body body = mainPart.Document.AppendChild(new Body());
                Paragraph para = body.AppendChild(new Paragraph());
                Run run = para.AppendChild(new Run());
                run.AppendChild(new Text(textToInsert));
            }

            if (openFile)
            { CommonUtilities.OpenWithDefaultProgram(fileFullName); }
        }


    }
}
