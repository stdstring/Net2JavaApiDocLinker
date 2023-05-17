using System;
using System.Collections.Generic;
using System.Windows.Forms;
using UserDocsApiResolver.Library;
using UserDocsApiResolver.Library.ApiMemberStorage;

namespace UserDocsApiResolver
{
    internal class UserRequestProcessor : IUserRequestProcessor
    {
        public UserRequestProcessor(ApiMemberStorageManager apiMemberStorageManager)
        {
            _apiMemberRequestProcessor = new ApiMemberRequestProcessor(apiMemberStorageManager);
        }

        public void Process(UserActionKeys key)
        {
            Clipboard.Clear();
            SendKeys.SendWait(CopyCommand);
            String clipboardText;
            TextDataFormat textDataFormat;
            if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                clipboardText = Clipboard.GetText(TextDataFormat.Text);
                textDataFormat = TextDataFormat.Text;
            }
            else if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
            {
                clipboardText = Clipboard.GetText(TextDataFormat.UnicodeText);
                textDataFormat = TextDataFormat.UnicodeText;
            }
            else
                return;
            String response = _apiMemberRequestProcessor.Process(clipboardText, _userActionMap[key]);
            Clipboard.SetText(response, textDataFormat);
            SendKeys.SendWait(PasteCommand);
        }

        private readonly ApiMemberRequestProcessor _apiMemberRequestProcessor;
        private readonly IDictionary<UserActionKeys, ApiMemberPlatform> _userActionMap =
            new Dictionary<UserActionKeys, ApiMemberPlatform>
                {
                    {UserActionKeys.CTRL_1_KEY, ApiMemberPlatform.Net},
                    {UserActionKeys.CTRL_2_KEY, ApiMemberPlatform.Java}
                };

        private const String CopyCommand = "^c";
        private const String PasteCommand = "^v";
    }
}
