using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Utilities;
using MudExtensions.Extensions;

namespace MudExtensions
{
    public partial class MudPasswordField<T> : MudDebouncedInput<T>
    {
        protected string Classname =>
           new CssBuilder("mud-input-input-control")
           .AddClass(Class)
           .Build();

        public MudInput<string> InputReference { get; private set; }
        InputType _passwordInput = InputType.Password;
        string _passwordIcon = Icons.Material.Filled.VisibilityOff;
        bool _passwordMode = true;

        [CascadingParameter(Name = "Standalone")]
        internal bool StandaloneEx { get; set; } = true;

        /// <summary>
        /// Type of the input element. It should be a valid HTML5 input type.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public InputType InputType { get; set; } = InputType.Text;

        private string GetCounterText() => Counter == null ? string.Empty : (Counter == 0 ? (string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") : ((string.IsNullOrEmpty(Text) ? "0" : $"{Text.Length}") + $" / {Counter}"));

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Clearable { get; set; } = false;

        /// <summary>
        /// Button click event for clear button. Called after text and value has been cleared.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClearButtonClick { get; set; }

        public override ValueTask FocusAsync()
        {
            return InputReference.FocusAsync();
        }

        public override ValueTask BlurAsync()
        {
            return InputReference.BlurAsync();
        }

        public override ValueTask SelectAsync()
        {
            return InputReference.SelectAsync();
        }

        public override ValueTask SelectRangeAsync(int pos1, int pos2)
        {
            return InputReference.SelectRangeAsync(pos1, pos2);
        }

        protected override void ResetValue()
        {
            InputReference.Reset();
            base.ResetValue();
        }

        /// <summary>
        /// Clear the text field, set Value to default(T) and Text to null
        /// </summary>
        /// <returns></returns>
        public Task Clear()
        {
            return InputReference.SetText(null);
        }

        /// <summary>
        /// Sets the input text from outside programmatically
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task SetText(string text)
        {
            if (InputReference != null)
                await InputReference.SetText(text);
        }

        private async Task OnMaskedValueChanged(string s)
        {
            await SetTextAsync(s);
        }

        [Parameter]
        public bool PasswordMode
        {
            get => _passwordMode;
            set
            {
                if (_passwordMode == value)
                {
                    return;
                }
                _passwordMode = value;
                if (_passwordMode == true)
                {
                    _passwordInput = InputType.Password;
                    _passwordIcon = Icons.Filled.VisibilityOff;
                }
                else
                {
                    _passwordInput = InputType.Text;
                    _passwordIcon = Icons.Filled.Visibility;
                }

                PasswordModeChanged.InvokeAsync(value).AndForgetExt();
            }
        }

        [Parameter]
        public EventCallback<bool> PasswordModeChanged { get; set; }

        protected async Task AdornmentClick()
        {
            PasswordMode = !PasswordMode;
            await OnAdornmentClick.InvokeAsync();
        }

    }
}
