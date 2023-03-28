﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using WingHinPortal.Module.BusinessObjects.PO;
using WingHinPortal.Module.BusinessObjects.Setup;
using WingHinPortal.Module.BusinessObjects.View;

namespace WingHinPortal.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppViewControllertopic.aspx.
    public partial class POView : ObjectViewController<ListView, PurchaseOrderDetails>
    {
        public POView()
        {
            InitializeComponent();
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
            NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
            if (controller != null)
            {
                //controller.NewObjectAction.Execute += NewObjectAction_Execute;
                controller.ObjectCreated += Controller_ObjectCreated;
            }
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
        protected override void OnDeactivated()
        {
            NewObjectViewController controller = Frame.GetController<NewObjectViewController>();
            if (controller != null)
            {
                //controller.NewObjectAction.Execute -= NewObjectAction_Execute;
                controller.ObjectCreated -= Controller_ObjectCreated;
            }
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void Controller_ObjectCreated(object sender, ObjectCreatedEventArgs e)
        {
            if (e.CreatedObject is PurchaseOrderDetails && View.IsRoot == false)
            {
                PurchaseOrderDetails currentObject = (PurchaseOrderDetails)e.CreatedObject;

                ListView lv = ((ListView)View);
                if (lv.CollectionSource is PropertyCollectionSource)
                {
                    PropertyCollectionSource collectionSource = (PropertyCollectionSource)lv.CollectionSource;
                    if (collectionSource.MasterObject != null)
                    {
                        if (collectionSource.MasterObjectType == typeof(PurchaseOrders))
                        {
                            PurchaseOrders masterobject = (PurchaseOrders)collectionSource.MasterObject;

                            if (masterobject.ExpenditureType != null)
                            {
                                currentObject.ExpenditureType = currentObject.Session.FindObject<ExpenditureType>(
                                    new BinaryOperator("ExpenditureTypeCode", masterobject.ExpenditureType.ExpenditureTypeCode, BinaryOperatorType.Equal));
                            }

                            if (masterobject.ItemGroup != null)
                            {
                                currentObject.ItemGroup = currentObject.Session.FindObject<vwItemGroup>(new BinaryOperator("Code", masterobject.ItemGroup.Code, BinaryOperatorType.Equal));
                            }
                        }
                    }
                }

            }
        }
    }
}
