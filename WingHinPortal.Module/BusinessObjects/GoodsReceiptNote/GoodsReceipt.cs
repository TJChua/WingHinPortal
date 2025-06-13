﻿using System;
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
using WingHinPortal.Module.BusinessObjects.Setup;
using DevExpress.ExpressApp.ConditionalAppearance;
using WingHinPortal.Module.BusinessObjects.View;

namespace WingHinPortal.Module.BusinessObjects.GoodsReceipt
{
    [DefaultClassOptions]
    [XafDisplayName("Goods Receipt Note")]
    [NavigationItem("Goods Receipt Note")]
    [DefaultProperty("DocNum")]
    [Appearance("HideEdit", AppearanceItemType.Action, "True", TargetItems = "SwitchToEditMode; Edit", Criteria = "not (DocStatus in (0))", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideDelete", AppearanceItemType.Action, "True", TargetItems = "Delete", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideSubmit", AppearanceItemType.Action, "True", TargetItems = "SubmitGRN", Criteria = "not (DocStatus in (0))", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]
    [Appearance("HideCancel", AppearanceItemType.Action, "True", TargetItems = "CancelGRN", Criteria = "not (DocStatus in (0))", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]

    [Appearance("HideCopyFrom", AppearanceItemType.Action, "True", TargetItems = "CopyFromPO", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "ListView")]
    [Appearance("HideCopyFrom1", AppearanceItemType.Action, "True", TargetItems = "CopyFromPO", Criteria = "not IsNew", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide, Context = "Any")]

    public class GoodsReceipt : XPObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument113146.aspx).
        public GoodsReceipt(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://documentation.devexpress.com/eXpressAppFramework/CustomDocument112834.aspx).
            SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
            CreateUser = Session.GetObjectByKey<SystemUsers>(user.Oid);
            CreateDate = DateTime.Now;
            DocDate = DateTime.Now;
            PostingDate = DateTime.Now;
            DocType = Session.FindObject<DocTypes>
                    (new BinaryOperator("BoCode", DocTypeList.GRN, BinaryOperatorType.Equal));
            if (user.Warehouse != null)
            {
                Warehouse = Session.FindObject<vwWarehouse>
                    (new BinaryOperator("BoCode", user.Warehouse.BoCode, BinaryOperatorType.Equal));
            }

            if (user.Staff != null)
            {
                Department = Session.FindObject<Department>
                    (new BinaryOperator("DepartmentCode", user.Staff.StaffDepartment.DepartmentCode, BinaryOperatorType.Equal));
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

        private DocTypes _DocType;
        [Appearance("DocType", Enabled = false, Criteria = "not IsNew")]
        [Index(304), VisibleInListView(false), VisibleInDetailView(false), VisibleInLookupListView(false)]
        public virtual DocTypes DocType
        {
            get { return _DocType; }
            set
            {
                SetPropertyValue("DocType", ref _DocType, value);
            }
        }

        private string _DocNum;
        [XafDisplayName("GRN No.")]
        [Appearance("DocNum", Enabled = false)]
        [Index(3), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        public string DocNum
        {
            get { return _DocNum; }
            set
            {
                SetPropertyValue("DocNum", ref _DocNum, value);
            }
        }

        private vwVendors _VendorCode;
        [NoForeignKey]
        [ImmediatePostData]
        [DataSourceCriteria("ValidFor = 'Y' and Expenditure = '@this.ExpenditureType.ExpenditureTypeCode'")]
        [XafDisplayName("Vendor Code")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(5), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        public vwVendors VendorCode
        {
            get { return _VendorCode; }
            set
            {
                SetPropertyValue("VendorCode", ref _VendorCode, value);
                if (!IsLoading && value != null)
                {
                    VendorName = VendorCode.CardName.ToUpper().ToString();
                }
                else if (!IsLoading && value == null)
                {
                    VendorName = null;
                }
            }
        }

        private string _VendorName;
        [XafDisplayName("Vendor Name.")]
        [Index(8), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        [Appearance("VendorName", Enabled = false)]
        public string VendorName
        {
            get { return _VendorName; }
            set
            {
                SetPropertyValue("VendorName", ref _VendorName, value);
            }
        }

        private Department _Department;
        [ImmediatePostData]
        //[DataSourceCriteria("(Entity.MasterData = '@this.Entity.MasterData')")]
        [Appearance("Department", Enabled = false)]
        [XafDisplayName("Department")]
        [Index(10), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        public Department Department
        {
            get { return _Department; }
            set
            {
                SetPropertyValue("Department", ref _Department, value);
            }
        }

        private vwWarehouse _Warehouse;
        [NoForeignKey]
        [ImmediatePostData]
        [DataSourceCriteria("Inactive = 'N'")]
        [XafDisplayName("Warehouse")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(11), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public vwWarehouse Warehouse
        {
            get { return _Warehouse; }
            set
            {
                SetPropertyValue("Warehouse", ref _Warehouse, value);
            }
        }

        private DateTime _DocDate;
        [XafDisplayName("Doc Date")]
        [Appearance("DocDate", Enabled = false)]
        [Index(13), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public DateTime DocDate
        {
            get { return _DocDate; }
            set
            {
                SetPropertyValue("DocDate", ref _DocDate, value);
            }
        }

        private DateTime _PostingDate;
        [XafDisplayName("Posting Date")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(15), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public DateTime PostingDate
        {
            get { return _PostingDate; }
            set
            {
                SetPropertyValue("PostingDate", ref _PostingDate, value);
            }
        }

        private DateTime _DeliveryDate;
        [XafDisplayName("Delivery Date")]
        [RuleRequiredField(DefaultContexts.Save)]
        [Index(16), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(false)]
        public DateTime DeliveryDate
        {
            get { return _DeliveryDate; }
            set
            {
                SetPropertyValue("DeliveryDate", ref _DeliveryDate, value);
            }
        }

        private DocStatus _DocStatus;
        [XafDisplayName("Doc Status")]
        [Appearance("DocStatus", Enabled = false)]
        [Index(18), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
        public DocStatus DocStatus
        {
            get { return _DocStatus; }
            set
            {
                SetPropertyValue("DocStatus", ref _DocStatus, value);
            }
        }

        private ExpenditureType _ExpenditureType;
        [ImmediatePostData]
        [DataSourceCriteria("IsActive = 'True'")]
        [XafDisplayName("Expenditure Type")]
        [Index(20), VisibleInDetailView(true), VisibleInListView(true), VisibleInLookupListView(true)]
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
        [Index(23), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public vwItemGroup ItemGroup
        {
            get { return _ItemGroup; }
            set
            {
                SetPropertyValue("ItemGroup", ref _ItemGroup, value);
            }
        }

        private CompanyAddress _CompanyAddress;
        [XafDisplayName("Delivery Address")]
        [Index(25), VisibleInListView(false), VisibleInDetailView(true), VisibleInLookupListView(false)]
        [RuleRequiredField(DefaultContexts.Save)]
        public CompanyAddress CompanyAddress
        {
            get { return _CompanyAddress; }
            set
            {
                SetPropertyValue("CompanyAddress", ref _CompanyAddress, value);
            }
        }

        private decimal _Total;
        [XafDisplayName("Total")]
        [Appearance("Total", Enabled = false)]
        [DbType("numeric(18,6)")]
        [ModelDefault("DisplayFormat", "{0:n2}")]
        [Index(80), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public decimal Total
        {
            get
            {
                if (Session.IsObjectsSaving != true)
                {
                    decimal rtn = 0;
                    if (GoodsReceiptDetails != null)
                        rtn += GoodsReceiptDetails.Sum(p => p.SubTotal);

                    return rtn;
                }
                else
                {
                    return _Total;
                }
            }
            set
            {
                SetPropertyValue("Total", ref _Total, value);
            }
        }

        private string _Remarks;
        [XafDisplayName("Remarks")]
        [Size(254)]
        [Index(81), VisibleInDetailView(true), VisibleInListView(false), VisibleInLookupListView(false)]
        public string Remarks
        {
            get { return _Remarks; }
            set
            {
                SetPropertyValue("Remarks", ref _Remarks, value);
            }
        }

        private bool _Sap;
        [XafDisplayName("Sap")]
        [Index(82), VisibleInDetailView(false), VisibleInListView(false), VisibleInLookupListView(false)]
        public bool Sap
        {
            get { return _Sap; }
            set
            {
                SetPropertyValue("Sap", ref _Sap, value);
            }
        }

        [Browsable(false)]
        public bool IsNew
        {
            get
            { return Session.IsNewObject(this); }
        }

        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                int count = 0;
                foreach (GoodsReceiptAttachment dtl in this.GoodsReceiptAttachment)
                {
                    count += 1;
                }

                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        [Association("GoodsReceipt-GoodsReceiptDetails")]
        [XafDisplayName("Items")]
        public XPCollection<GoodsReceiptDetails> GoodsReceiptDetails
        {
            get { return GetCollection<GoodsReceiptDetails>("GoodsReceiptDetails"); }
        }

        [Association("GoodsReceipt-GoodsReceiptDocStatus")]
        [XafDisplayName("Document Status")]
        public XPCollection<GoodsReceiptDocStatus> GoodsReceiptDocStatus
        {
            get { return GetCollection<GoodsReceiptDocStatus>("GoodsReceiptDocStatus"); }
        }

        [Association("GoodsReceipt-GoodsReceiptAttachment")]
        [XafDisplayName("Attachments")]
        public XPCollection<GoodsReceiptAttachment> GoodsReceiptAttachment
        {
            get { return GetCollection<GoodsReceiptAttachment>("GoodsReceiptAttachment"); }
        }

        private XPCollection<AuditDataItemPersistent> auditTrail;
        public XPCollection<AuditDataItemPersistent> AuditTrail
        {
            get
            {
                if (auditTrail == null)
                {
                    auditTrail = AuditedObjectWeakReference.GetAuditTrail(Session, this);
                }
                return auditTrail;
            }
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (!(Session is NestedUnitOfWork)
                && (Session.DataLayer != null)
                    && (Session.ObjectLayer is SimpleObjectLayer)
                        )
            {
                SystemUsers user = (SystemUsers)SecuritySystem.CurrentUser;
                if (user != null)
                {
                    UpdateUser = Session.GetObjectByKey<SystemUsers>(user.Oid);
                }
                UpdateDate = DateTime.Now;

                if (Session.IsNewObject(this))
                {
                    GoodsReceiptDocStatus ds = new GoodsReceiptDocStatus(Session);
                    ds.DocStatus = DocStatus.New;
                    ds.DocRemarks = "";
                    ds.CreateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    ds.CreateDate = DateTime.Now;
                    ds.UpdateUser = Session.GetObjectByKey<SystemUsers>(SecuritySystem.CurrentUserId);
                    ds.UpdateDate = DateTime.Now;
                    this.GoodsReceiptDocStatus.Add(ds);
                }
            }
        }
    }
}