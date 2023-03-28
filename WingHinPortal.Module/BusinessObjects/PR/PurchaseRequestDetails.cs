using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp.ConditionalAppearance;
using WingHinPortal.Module.BusinessObjects.Setup;
using WingHinPortal.Module.BusinessObjects.View;

namespace WingHinPortal.Module.BusinessObjects.PR
{
    [DefaultClassOptions]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("LinkDoc", AppearanceItemType = "Action", TargetItems = "Link", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [Appearance("UnlinkDoc", AppearanceItemType = "Action", TargetItems = "Unlink", Context = "ListView", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide)]
    [XafDisplayName("Purchase Request Details")]
    public class PurchaseRequestDetails : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public PurchaseRequestDetails(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
            CreateDate = DateTime.Now;

            //Tax = Session.FindObject<vwTax>(new BinaryOperator("BoCode", "X0"));
            Quantity = 1;
            if (CreateUser.Staff.CostCenter != null)
            {
                CostCenter = Session.FindObject<vwCostCenter>(new BinaryOperator("PrcCode", CreateUser.Staff.CostCenter.PrcCode));
            }
        }

        private SystemUsers _CreateUser;
        [XafDisplayName("Create User")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(300), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public SystemUsers CreateUser
        {
            get { return _CreateUser; }
            set
            {
                SetPropertyValue("CreateUser", ref _CreateUser, value);
            }
        }

        private DateTime? _CreateDate;
        [Index(301), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public DateTime? CreateDate
        {
            get { return _CreateDate; }
            set
            {
                SetPropertyValue("CreateDate", ref _CreateDate, value);
            }
        }

        private SystemUsers _UpdateUser;
        [XafDisplayName("Update User"), ToolTip("Enter Text")]
        //[ModelDefault("EditMask", "(000)-00"), VisibleInListView(false)]
        [Index(302), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public SystemUsers UpdateUser
        {
            get { return _UpdateUser; }
            set
            {
                SetPropertyValue("UpdateUser", ref _UpdateUser, value);
            }
        }

        private DateTime? _UpdateDate;
        [Index(303), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public DateTime? UpdateDate
        {
            get { return _UpdateDate; }
            set
            {
                SetPropertyValue("UpdateDate", ref _UpdateDate, value);
            }
        }

        private vwItemMasters _Item;
        [ImmediatePostData]
        [NoForeignKey]
        [DataSourceCriteria("frozenFor = 'N' and U_EXPENDITURETYPE = '@this.ExpenditureType.ExpenditureTypeCode' and " +
            "U_ItemGroup = '@this.ItemGroup.Code'")]
        [XafDisplayName("Item")]
        [Index(0), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        [Appearance("Item", Enabled = false, Criteria = "not IsNew")]
        [RuleRequiredField(DefaultContexts.Save)]
        public vwItemMasters Item

        {
            get { return _Item; }
            set
            {
                SetPropertyValue("Item", ref _Item, value);
                if (!IsLoading && value != null)
                {
                    ItemDesc = Item.ItemName;
                    Tax = Session.FindObject<vwTax>(CriteriaOperator.Parse("BoCode = ?", Item.PuchaseTax));

                    vwSupplierPrice tempprice;
                    tempprice = Session.FindObject<vwSupplierPrice>(CriteriaOperator.Parse("ItemCode = ?", Item.ItemCode));

                    if (tempprice != null)
                    {
                        Unitprice = tempprice.Price;
                    }
                }
                else if (!IsLoading && value == null)
                {
                    ItemDesc = null;
                    Unitprice = 0;
                    Tax = null;
                }
            }
        }

        private string _ItemDesc;
        [RuleRequiredField(DefaultContexts.Save)]
        [XafDisplayName("Item Description")]
        [Appearance("ItemDesc", Enabled = false)]
        [Index(3), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public string ItemDesc
        {
            get { return _ItemDesc; }
            set
            {
                SetPropertyValue("ItemDesc", ref _ItemDesc, value);
            }
        }

        private decimal _Quantity;
        [ImmediatePostData]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [XafDisplayName("Quantity")]
        [Index(5), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal Quantity
        {
            get { return _Quantity; }
            set
            {
                SetPropertyValue("Quantity", ref _Quantity, value);
                if (!IsLoading)
                {
                    TaxAmount = (Quantity * Unitprice) * (TaxRate / 100);
                    SubTotalWithoutTax = Quantity * Unitprice;
                    SubTotal = Quantity * Unitprice + TaxAmount;
                    OpenQuantity = Quantity;
                }
            }
        }

        private decimal _OpenQuantity;
        [ImmediatePostData]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [XafDisplayName("Open Quantity")]
        [Appearance("OpenQuantity", Enabled = false)]
        [Index(8), VisibleInListView(true), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public decimal OpenQuantity
        {
            get { return _OpenQuantity; }
            set
            {
                SetPropertyValue("OpenQuantity", ref _OpenQuantity, value);
            }
        }

        private decimal _Unitprice;
        [ImmediatePostData]
        [XafDisplayName("Unit Price")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Index(10), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal Unitprice
        {
            get { return _Unitprice; }
            set
            {
                SetPropertyValue("Unitprice", ref _Unitprice, value);
                if (!IsLoading)
                {
                    TaxAmount = (Quantity * Unitprice) * (TaxRate / 100);
                    SubTotalWithoutTax = Quantity * Unitprice;
                    SubTotal = Quantity * Unitprice + TaxAmount;
                }
            }
        }

        private vwTax _Tax;
        [NoForeignKey]
        [ImmediatePostData]
        [XafDisplayName("Tax")]
        [Index(13), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        public vwTax Tax
        {
            get { return _Tax; }
            set
            {
                SetPropertyValue("Tax", ref _Tax, value);
                if (!IsLoading && value != null)
                {
                    TaxRate = Tax.Rate;
                }
                else if (!IsLoading && value == null)
                {
                    TaxRate = 0;
                }
            }
        }

        private decimal _TaxRate;
        [ImmediatePostData]
        [XafDisplayName("Tax Rate")]
        [Appearance("TaxRate", Enabled = false)]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Index(15), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        public decimal TaxRate
        {
            get { return _TaxRate; }
            set
            {
                SetPropertyValue("TaxRate", ref _TaxRate, value);
                if (!IsLoading && value != 0)
                {
                    TaxAmount = (Quantity * Unitprice) * (TaxRate / 100);
                    SubTotalWithoutTax = Quantity * Unitprice;
                    SubTotal = Quantity * Unitprice + TaxAmount;
                }
            }
        }

        private decimal _TaxAmount;
        [XafDisplayName("Tax Amount")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Appearance("TaxAmount", Enabled = false)]
        [Index(18), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        public decimal TaxAmount
        {
            get { return _TaxAmount; }
            set
            {
                SetPropertyValue("TaxAmount", ref _TaxAmount, value);
                //if (!IsLoading && value != 0)
                //{
                //    TaxAmount = (Quantity * Unitprice - TotalDiscount) * (TaxRate / 100);
                //    SubTotalWithoutTax = Quantity * Unitprice - TotalDiscount;
                //    SubTotal = Quantity * Unitprice - TotalDiscount + TaxAmount;
                //}
            }
        }

        private decimal _SubTotalWithoutTax;
        [XafDisplayName("SubTotal w/o Tax")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Appearance("SubTotalWithoutTax", Enabled = false)]
        [Index(20), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        public decimal SubTotalWithoutTax
        {
            get { return _SubTotalWithoutTax; }
            set
            {
                SetPropertyValue("SubTotalWithoutTax", ref _SubTotalWithoutTax, value);
            }
        }

        private decimal _SubTotal;
        [XafDisplayName("SubTotal")]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Appearance("SubTotal", Enabled = false)]
        [Index(23), VisibleInListView(true), VisibleInDetailView(true), VisibleInLookupListView(true)]
        public decimal SubTotal
        {
            get { return _SubTotal; }
            set
            {
                SetPropertyValue("SubTotal", ref _SubTotal, value);
            }
        }

        private ExpenditureType _ExpenditureType;
        [ImmediatePostData]
        [XafDisplayName("ExpenditureType")]
        [Index(25), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public ExpenditureType ExpenditureType
        {
            get { return _ExpenditureType; }
            set
            {
                SetPropertyValue("ExpenditureType", ref _ExpenditureType, value);
            }
        }

        private vwItemGroup _ItemGroup;
        [NoForeignKey]
        [ImmediatePostData]
        [XafDisplayName("ItemGroup")]
        [RuleRequiredField(DefaultContexts.Save)]
        [DataSourceCriteria("Expenditure = '@this.ExpenditureType.ExpenditureTypeCode'")]
        [Index(28), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public vwItemGroup ItemGroup
        {
            get { return _ItemGroup; }
            set
            {
                SetPropertyValue("ItemGroup", ref _ItemGroup, value);
            }
        }

        private vwCostCenter _CostCenter;
        [NoForeignKey]
        [ImmediatePostData]
        [XafDisplayName("CostCenter")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(30), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public vwCostCenter CostCenter
        {
            get { return _CostCenter; }
            set
            {
                SetPropertyValue("CostCenter", ref _CostCenter, value);
            }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        private PurchaseRequest _PurchaseRequest;
        [Association("PurchaseRequest-PurchaseRequestDetails")]
        [Index(99), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        [Appearance("PurchaseRequest", Enabled = false)]
        public PurchaseRequest PurchaseRequest
        {
            get { return _PurchaseRequest; }
            set { SetPropertyValue("PurchaseRequest", ref _PurchaseRequest, value); }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (!(Session is NestedUnitOfWork)
                && (Session.DataLayer != null)
                    && (Session.ObjectLayer is SimpleObjectLayer)
                        )
            {
                UpdateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                UpdateDate = DateTime.Now;

                if (Session.IsNewObject(this))
                {

                }
            }
        }
    }
}