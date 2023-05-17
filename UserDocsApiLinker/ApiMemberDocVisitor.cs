using Aspose.Words;

namespace UserDocsApiLinker
{
    /// <summary>
    /// Class for implementation DocumentVisitor pattern.
    /// </summary>
    internal class ApiMemberDocVisitor : DocumentVisitor
    {
        /// <summary>
        /// R1.2 Joining sames Runs.
        /// </summary>
        /// <param name="paragraph">paragraph, where need to joining Runs with the same Style</param>
        public override VisitorAction VisitParagraphStart(Paragraph paragraph)
        {
            int i = 0;
            Run currentRun, previousRun;
            while (i < paragraph.Runs.Count)
            {
                currentRun = paragraph.Runs[i];
                if (IsStyleSets(currentRun, "ApiLink"))
                {
                    previousRun = (Run)currentRun.PreviousSibling;
                    if ((previousRun != null) && (IsStyleSets(previousRun, "ApiLink")))
                    {
                        currentRun.Text = previousRun.Text + currentRun.Text;
                        previousRun.Remove();
                        continue;
                    }
                }
                if (!IsStyleSets(currentRun, "ApiLink") && (currentRun.PreviousSibling != null) && (currentRun.PreviousSibling.NodeType == NodeType.Run))
                {
                    previousRun = (Run)currentRun.PreviousSibling;
                    if ((IsStyleSets(previousRun, "ApiLink")) && (previousRun.PreviousSibling != null) && (previousRun.PreviousSibling.NodeType == NodeType.Run))
                    {
                        Run prevPreviousRun = (Run)previousRun.PreviousSibling;
                        if ((prevPreviousRun != null) && (!IsStyleSets(prevPreviousRun, "ApiLink")))
                        {
                            TrimWhitespases(prevPreviousRun, previousRun, currentRun);
                        }
                    }
                }
                
                i++;
            }

            return VisitorAction.Continue;
        }

        private static bool IsStyleSets(Run run, string style)
        {
            return run.Font.Style.Name == style;
        }

        private static void TrimWhitespases(Run prevRun, Run targetRun, Run nextRun)
        {
            if (string.IsNullOrEmpty(targetRun.Text.Trim()))
                return;

            if (targetRun.Text.Substring(0, 1) == " ")
            {
                targetRun.Text = targetRun.Text.TrimStart(' ');
                if(prevRun.Text.Substring(prevRun.Text.Length-1) != " ")
                    prevRun.Text = string.Format("{0} ", prevRun.Text);
            }
            if (targetRun.Text.Substring(targetRun.Text.Length - 1) == " ")
            {
                targetRun.Text = targetRun.Text.TrimEnd(' ');
                if(nextRun.Text.Substring(0, 1) != " ")
                    nextRun.Text = string.Format(" {0}", nextRun.Text);
            }
        }
    }
}
