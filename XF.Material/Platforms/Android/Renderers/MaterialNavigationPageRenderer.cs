﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XF.Material.Droid.Renderers;
using XF.Material.Forms.UI;
using Toolbar = AndroidX.AppCompat.Widget.Toolbar;

[assembly: ExportRenderer(typeof(MaterialNavigationPage), typeof(MaterialNavigationPageRenderer))]

namespace XF.Material.Droid.Renderers
{
    public class MaterialNavigationPageRenderer : Xamarin.Forms.Platform.Android.AppCompat.NavigationPageRenderer
    {
        private MaterialNavigationPage _navigationPage;
        private Toolbar _toolbar;
        private Page _childPage;

        public MaterialNavigationPageRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<NavigationPage> e)
        {
            base.OnElementChanged(e);

            if (e?.NewElement != null)
            {
                _navigationPage = Element as MaterialNavigationPage;

                _toolbar = ViewGroup.GetChildAt(0) as Toolbar;

                HandleChildPage(_navigationPage.CurrentPage);
            }

            if (e?.OldElement != null && _childPage != null)
            {
                _childPage.PropertyChanged -= ChildPage_PropertyChanged;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == NavigationPage.CurrentPageProperty.PropertyName)
            {
                HandleChildPage(_navigationPage.CurrentPage);
            }
        }

        private void HandleChildPage(Page page)
        {
            if (_childPage != null)
            {
                _childPage.PropertyChanged -= ChildPage_PropertyChanged;
            }

            _childPage = page;

            if (_childPage != null)
            {
                _childPage.PropertyChanged += ChildPage_PropertyChanged;
            }
        }

        private void ChildPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var page = sender as Page;

            if(page == null)
            {
                return;
            }

            if (e.PropertyName == MaterialNavigationPage.AppBarElevationProperty.PropertyName)
            {
                ChangeElevation(page);
            }
        }

        protected override Task<bool> OnPopToRootAsync(Page page, bool animated)
        {
            _navigationPage.InternalPopToRoot(page);

            ChangeElevation(page);

            return base.OnPopToRootAsync(page, animated);
        }

        protected override Task<bool> OnPopViewAsync(Page page, bool animated)
        {
            var navStack = _navigationPage.Navigation.NavigationStack.ToList();

            if (navStack.Count - 1 - navStack.IndexOf(page) < 0)
            {
                return base.OnPopViewAsync(page, animated);
            }

            var previousPage = navStack[navStack.IndexOf(page) - 1];
            _navigationPage.InternalPagePop(previousPage, page);
            ChangeElevation(previousPage);

            return base.OnPopViewAsync(page, animated);
        }

        protected override Task<bool> OnPushAsync(Page page, bool animated)
        {
            _navigationPage.InternalPagePush(page);

            ChangeElevation(page);

            return base.OnPushAsync(page, animated);
        }

        private void ChangeElevation(Page page)
        {
            var elevation = (double)page.GetValue(MaterialNavigationPage.AppBarElevationProperty);

            ChangeElevation(elevation);
        }

        public void ChangeElevation(double elevation)
        {
            if (elevation > 0)
            {
                _toolbar.Elevate(elevation);
            }
            else
            {
                _toolbar.Elevate(0);
            }
        }
    }
}
