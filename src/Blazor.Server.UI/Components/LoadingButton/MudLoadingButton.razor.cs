using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Utilities;
using MudExtensions.Enums;
using MudExtensions.Extensions;

namespace MudExtensions
{
    public partial class MudLoadingButton : MudBaseButton
    {

        bool _loading = true;
        /// <summary>
        /// Two way binded loading state.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Loading 
        { 
            get => _loading; 
            set
            {
                if (_loading == value)
                {
                    return;
                }
                _loading = value;
                LoadingChanged.InvokeAsync(_loading).AndForgetExt();
            }
        }

        /// <summary>
        /// Fires when loading changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> LoadingChanged { get; set; }

        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color LoadingCircularColor { get; set; } = Color.Default;

        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Adornment LoadingAdornment { get; set; } = Adornment.Start;

        /// <summary>
        /// Icon placed before the text if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string StartIcon { get; set; }

        /// <summary>
        /// Icon placed before the text if set. Only works for IconButton variant. For button variant use Start and EndIcon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string Icon { get; set; }

        /// <summary>
        /// Icon placed after the text if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string EndIcon { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Color IconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// Icon class names, separated by space
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public string IconClass { get; set; }

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The Size of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public ButtonVariant ButtonVariant { get; set; } = ButtonVariant.Button;

        /// <summary>
        /// Title of the icon used for accessibility. Only for IconButton and Fab variants.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string Title { get; set; }

        /// <summary>
        /// The variant to use.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// If true, the button will take up 100% of available width.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public bool FullWidth { get; set; }

        /// <summary>
        /// Custom loader content. If it is set, the overlap, darken and loadertype parameters ignored.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public RenderFragment LoadingContent { get; set; }

        /// <summary>
        /// The child content.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// The size of the icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// If applied the text will be added to the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string Label { get; set; }

        /// <summary>
        /// If not null, LoadingButton goes for loading state for determined miliseconds.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public int? AutoDelay { get; set; } = 300;

        protected async Task ButtonClick(MouseEventArgs args)
        {
            
            if (AutoDelay != null)
            {
                Task task = Task.Delay(AutoDelay.Value);
                _loading = true;
                await OnClickHandler(args);
                await task;
                _loading = false;
            }
            else
            {
                await OnClickHandler(args);
            }
        }

    }
}
