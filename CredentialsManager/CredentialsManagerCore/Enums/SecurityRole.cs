using System.Runtime.Serialization;

namespace Draeger.Testautomation.CredentialsManagerCore
{
    [DataContract]
    public enum SecurityRole
    {
        Base = 0,

        [EnumMember(Value = "Activity Feeds")]
        ActivityFeeds,

        [EnumMember(Value = "AM DUMMY ROLE")]
        AmDummyRole,

        [EnumMember(Value = "Common Data Service User")]
        CommonDataServiceUser,

        [EnumMember(Value = "Crowe EditAble CRM Grid Administrator")]
        CroweEditAbleCrmGridAdministrator,

        [EnumMember(Value = "Crowe EditAble CRM Grid User")]
        CroweEditAbleCrmGridUser,

        [EnumMember(Value = "Customer service app access")]
        CustomerServiceAppAccess,

        [EnumMember(Value = "Delegate")]
        Delegate,

        [EnumMember(Value = "Draeger_Default_Team_Role")]
        DraegerDefaultTeamRole,

        [EnumMember(Value = "Draeger_General_Reference_ Data_Role")]
        DraegerGeneralReferenceDataRole,

        [EnumMember(Value = "Draeger_Global_Testcard_Role")]
        DraegerGlobalTestcardRole,

        [EnumMember(Value = "Draeger_Language_Role")]
        DraegerLanguageRole,

        [EnumMember(Value = "Draeger_Local_Business_Administrator_Role")]
        DraegerLocalBusinessAdministratorRole,

        [EnumMember(Value = "Draeger_Local_Reference_Data_Role")]
        DraegerLocalReferenceDataRole,

        [EnumMember(Value = "Draeger_Local_Testcard_Admin_Role")]
        DraegerLocalTestcardAdminRole,

        [EnumMember(Value = "Draeger_Maintenance_wo_Contract_Role")]
        DraegerMaintenanceWoContractRole,

        [EnumMember(Value = "Draeger_Reporting_Role")]
        DraegerReportingRole,

        [EnumMember(Value = "Draeger_Service_Backoffice_Role")]
        DraegerServiceBackofficeRole,

        [EnumMember(Value = "Draeger_Service_Basic_Role")]
        DraegerServiceBasicRole,

        [EnumMember(Value = "Draeger_Service_Cross_Border_Role")]
        DraegerServiceCrossBorderRole,

        [EnumMember(Value = "Draeger_Service_Dispatcher_Role")]
        DraegerServiceDispatcherRole,

        [EnumMember(Value = "Draeger_Service_ERP_Activity_Code_Role")]
        DraegerServiceErpActivityCodeRole,

        [EnumMember(Value = "Draeger_Service_ERP_Cost_Estimation_Request")]
        DraegerServiceErpCostEstimationRequest,

        [EnumMember(Value = "Draeger_Service_ERP_Pro_Forma_Role")]
        DraegerServiceErpProFormaRole,

        [EnumMember(Value = "Draeger_Service_Extended_Reading_Role")]
        DraegerServiceExtendedReadingRole,

        [EnumMember(Value = "Draeger_Service_Extended_Writing_Role")]
        DraegerServiceExtendedWritingRole,

        [EnumMember(Value = "Draeger_Service_Gatekeeper_Installed Base_Role")]
        DraegerServiceGatekeeperInstalledBaseRole,

        [EnumMember(Value = "Draeger_Service_Gatekeeper_Instition_Role")]
        DraegerServiceGatekeeperInstitionRole,

        [EnumMember(Value = "Draeger_Service_Global_Key_User_Role")]
        DraegerServiceGlobalKeyUserRole,

        [EnumMember(Value = "Draeger_Service_Manager_Role")]
        DraegerServiceManagerRole,

        [EnumMember(Value = "Draeger_Service_NAV_Advanced_Edit_Role")]
        DraegerServiceNavAdvancedEditRole,

        [EnumMember(Value = "Draeger_Service_QM_Reading_Role")]
        DraegerServiceQmReadingRole,

        [EnumMember(Value = "Draeger_Service_SAP_Cost_Estimation_Role")]
        DraegerServiceSapCostEstimationRole,

        [EnumMember(Value = "Draeger_Service_Sharing_Role")]
        DraegerServiceSharingRole,

        [EnumMember(Value = "Draeger_Service_Start_Dialog_Role")]
        DraegerServiceStartDialogRole,

        [EnumMember(Value = "Draeger_Service_Technician_Role")]
        DraegerServiceTechnicianRole,

        [EnumMember(Value = "Draeger_Service_Tools_Manager")]
        DraegerServiceToolsManager,

        [EnumMember(Value = "Draeger_Service_Tools_Read")]
        DraegerServiceToolsRead,

        [EnumMember(Value = "Draeger_Service_Tools_Technician")]
        DraegerServiceToolsTechnician,

        [EnumMember(Value = "Draeger_Service_Transfer_File")]
        DraegerServiceTransferFile,

        [EnumMember(Value = "Draeger_Service_Workflow Execution")]
        DraegerServiceWorkflowExecution,

        [EnumMember(Value = "Draeger_Serviceorder_without_Equipment_Role")]
        DraegerServiceorderWithoutEquipmentRole,

        [EnumMember(Value = "Draeger_System_Basic_Role")]
        DraegerSystemBasicRole,

        [EnumMember(Value = "DW Activity Extended Edit (BU)")]
        DwActivityExtendedEditBu,

        [EnumMember(Value = "DW Activity Extended Edit (P/C)")]
        DwActivityExtendedEditPC,

        [EnumMember(Value = "DW Activity Extended Read (BU)")]
        DwActivityExtendedReadBu,

        [EnumMember(Value = "DW Activity Extended Read (P/C)")]
        DwActivityExtendedReadPC,

        [EnumMember(Value = "DW App for Outlook")]
        DwAppForOutlook,

        [EnumMember(Value = "DW Associated Staff")]
        DwAssociatedStaff,

        [EnumMember(Value = "DW Basic CRM Access")]
        DwBasicCrmAccess,

        [EnumMember(Value = "DW Basic CRM Access Light")]
        DwBasicCrmAccessLight,

        [EnumMember(Value = "DW Basic CRM Access Light FW Testing")]
        DwBasicCrmAccessLightFwTesting,

        [EnumMember(Value = "DW Basic CRM Access Limited Collaboration")]
        DwBasicCrmAccessLimitedCollaboration,

        [EnumMember(Value = "DW Basic CRM Access Team Member")]
        DwBasicCrmAccessTeamMember,

        [EnumMember(Value = "DW Basic CRM Access Test Sec Role Visibility")]
        DwBasicCrmAccessTestSecRoleVisibility,

        [EnumMember(Value = "DW Bulk Edit")]
        DwBulkEdit,

        [EnumMember(Value = "DW Buying Center")]
        DwBuyingCenter,

        [EnumMember(Value = "DW Buying Center Extended Edit BU")]
        DwBuyingCenterExtendedEditBu,

        [EnumMember(Value = "DW Buying Center Extended Read P/C")]
        DwBuyingCenterExtendedReadPC,

        [EnumMember(Value = "DW Campaign Extended Edit (BU)")]
        DwCampaignExtendedEditBu,

        [EnumMember(Value = "DW Campaign Extended Edit (P/C)")]
        DwCampaignExtendedEditPC,

        [EnumMember(Value = "DW Campaign Extended Read (P/C)")]
        DwCampaignExtendedReadPC,

        [EnumMember(Value = "DW Campaign Management")]
        DwCampaignManagement,

        [EnumMember(Value = "DW Case Extended Edit (BU)")]
        DwCaseExtendedEditBu,

        [EnumMember(Value = "DW Case Extended Edit (P/C)")]
        DwCaseExtendedEditPC,

        [EnumMember(Value = "DW Case Management")]
        DwCaseManagement,

        [EnumMember(Value = "DW Case Read only (BU)")]
        DwCaseReadOnlyBu,

        [EnumMember(Value = "DW Case Read only (P/C)")]
        DwCaseReadOnlyPC,

        [EnumMember(Value = "DW Contact Extended Edit (BU)")]
        DwContactExtendedEditBu,

        [EnumMember(Value = "DW Contact Extended Edit (P/C)")]
        DwContactExtendedEditPC,

        [EnumMember(Value = "DW Contact Extended Read (BU)")]
        DwContactExtendedReadBu,

        [EnumMember(Value = "DW Contact Extended Read (P/C)")]
        DwContactExtendedReadPC,

        [EnumMember(Value = "DW Contract Read Only (BU)")]
        DwContractReadOnlyBu,

        [EnumMember(Value = "DW Contract Read Only (P/C)")]
        DwContractReadOnlyPC,

        [EnumMember(Value = "DW Customer Solution Extended Edit (BU)")]
        DwCustomerSolutionExtendedEditBu,

        [EnumMember(Value = "DW Customer Solution Extended Edit (P/C)")]
        DwCustomerSolutionExtendedEditPC,

        [EnumMember(Value = "DW Dispatch Read only (BU)")]
        DwDispatchReadOnlyBu,

        [EnumMember(Value = "DW Dispatch Read only (P/C)")]
        DwDispatchReadOnlyPC,

        [EnumMember(Value = "DW Document Template (O)")]
        DwDocumentTemplateO,

        [EnumMember(Value = "DW Document Template Execution (O)")]
        DwDocumentTemplateExecutionO,

        [EnumMember(Value = "DW E-mail Template (BU)")]
        DwEmailTemplateBu,

        [EnumMember(Value = "DW E-mail Template (P/C)")]
        DwEmailTemplatePC,

        [EnumMember(Value = "DW E-mail Template (User)")]
        DwEmailTemplateUser,

        [EnumMember(Value = "DW Equipment History Read only (BU)")]
        DwEquipmentHistoryReadOnlyBu,

        [EnumMember(Value = "DW Equipment History Read only (P/C)")]
        DwEquipmentHistoryReadOnlyPC,

        [EnumMember(Value = "DW Export")]
        DwExport,

        [EnumMember(Value = "DW Field Action Read Only (BU)")]
        DwFieldActionReadOnlyBu,

        [EnumMember(Value = "DW Field Action Read Only (P/C)")]
        DwFieldActionReadOnlyPC,

        [EnumMember(Value = "DW Financial Data Read (BU)")]
        DwFinancialDataReadBu,

        [EnumMember(Value = "DW Financial Data Read (P/C)")]
        DwFinancialDataReadPC,

        [EnumMember(Value = "DW Gatekeeper")]
        DwGatekeeper,

        [EnumMember(Value = "DW Global CRM Team")]
        DwGlobalCrmTeam,

        [EnumMember(Value = "DW Global Master Data Maintenance")]
        DwGlobalMasterDataMaintenance,

        [EnumMember(Value = "DW Go offline")]
        DwGoOffline,

        [EnumMember(Value = "DW Goal Create (User)")]
        DwGoalCreateUser,

        [EnumMember(Value = "DW Goal Management (BU)")]
        DwGoalManagementBu,

        [EnumMember(Value = "DW Goal Management (P/C)")]
        DwGoalManagementPC,

        [EnumMember(Value = "DW Goal Metrics Management (O)")]
        DwGoalMetricsManagementO,

        [EnumMember(Value = "DW Goal Read only (BU)")]
        DwGoalReadOnlyBu,

        [EnumMember(Value = "DW Goal Read only (User)")]
        DwGoalReadOnlyUser,

        [EnumMember(Value = "DW IB Sales Extended Edit (BU)")]
        DwIbSalesExtendedEditBu,

        [EnumMember(Value = "DW IB Sales Extended Edit (P/C)")]
        DwIbSalesExtendedEditPC,

        [EnumMember(Value = "DW IB Sales Management")]
        DwIbSalesManagement,

        [EnumMember(Value = "DW IB Sales Read Only (BU)")]
        DwIbSalesReadOnlyBu,

        [EnumMember(Value = "DW IB Sales Read Only (P/C)")]
        DwIbSalesReadOnlyPC,

        [EnumMember(Value = "DW Import Right")]
        DwImportRight,

        [EnumMember(Value = "DW Inco- and Payment Terms Read (O)")]
        DwIncoAndPaymentTermsReadO,

        [EnumMember(Value = "DW Installed Base Extended Edit (P/C)")]
        DwInstalledBaseExtendedEditPC,

        [EnumMember(Value = "DW Installed Base Extended Read (P/C)")]
        DwInstalledBaseExtendedReadPC,

        [EnumMember(Value = "DW Institution Create")]
        DwInstitutionCreate,

        [EnumMember(Value = "DW Institution Extended Edit (BU)")]
        DwInstitutionExtendedEditBu,

        [EnumMember(Value = "DW Institution Extended Edit (P/C)")]
        DwInstitutionExtendedEditPC,

        [EnumMember(Value = "DW Institution Extended Read (BU)")]
        DwInstitutionExtendedReadBu,

        [EnumMember(Value = "DW Institution Extended Read (P/C)")]
        DwInstitutionExtendedReadPC,

        [EnumMember(Value = "DW Language (O)")]
        DwLanguageO,

        [EnumMember(Value = "DW Lead Extended Edit (BU)")]
        DwLeadExtendedEditBu,

        [EnumMember(Value = "DW Lead Extended Edit (P/C)")]
        DwLeadExtendedEditPC,

        [EnumMember(Value = "DW Lead Extended Read (BU)")]
        DwLeadExtendedReadBu,

        [EnumMember(Value = "DW Lead Extended Read (P/C)")]
        DwLeadExtendedReadPC,

        [EnumMember(Value = "DW Mail Merge Template")]
        DwMailMergeTemplate,

        [EnumMember(Value = "DW Mail Merge Template Read (BU)")]
        DwMailMergeTemplateReadBu,

        [EnumMember(Value = "DW Mail Merge Template Read (P/C)")]
        DwMailMergeTemplateReadPC,

        [EnumMember(Value = "DW Manage User Sync Filters")]
        DwManageUserSyncFilters,

        [EnumMember(Value = "DW Marketing List Extended Edit (P/C)")]
        DwMarketingListExtendedEditPC,

        [EnumMember(Value = "DW Marketing List Extended Read (P/C)")]
        DwMarketingListExtendedReadPC,

        [EnumMember(Value = "DW Merge")]
        DwMerge,

        [EnumMember(Value = "DW Migration CRM 365")]
        DwMigrationCrm365,

        [EnumMember(Value = "DW Note Create (P/C)")]
        DwNoteCreatePC,

        [EnumMember(Value = "DW Note Extended Edit (BU)")]
        DwNoteExtendedEditBu,

        [EnumMember(Value = "DW Note Extended Edit (P/C)")]
        DwNoteExtendedEditPC,

        [EnumMember(Value = "DW Note Extended Read (BU)")]
        DwNoteExtendedReadBu,

        [EnumMember(Value = "DW Note Extended Read (P/C)")]
        DwNoteExtendedReadPC,

        [EnumMember(Value = "DW Opportunity Extended Edit (BU)")]
        DwOpportunityExtendedEditBu,

        [EnumMember(Value = "DW Opportunity Extended Edit (P/C)")]
        DwOpportunityExtendedEditPC,

        [EnumMember(Value = "DW Opportunity Extended Read (BU)")]
        DwOpportunityExtendedReadBu,

        [EnumMember(Value = "DW Opportunity Extended Read (P/C)")]
        DwOpportunityExtendedReadPC,

        [EnumMember(Value = "DW Opportunity Management")]
        DwOpportunityManagement,

        [EnumMember(Value = "DW Post Extended Create (O)")]
        DwPostExtendedCreateO,

        [EnumMember(Value = "DW Print")]
        DwPrint,

        [EnumMember(Value = "DW Quote follow-up")]
        DwQuoteFollowUp,

        [EnumMember(Value = "DW Quote follow-up Assign (P/C)")]
        DwQuoteFollowUpAssignPC,

        [EnumMember(Value = "DW Quote follow-up Configuration Read only (O)")]
        DwQuoteFollowUpConfigurationReadOnlyO,

        [EnumMember(Value = "DW Quote follow-up Create (BU)")]
        DwQuoteFollowUpCreateBu,

        [EnumMember(Value = "DW Quote follow-up Extended Edit (BU)")]
        DwQuoteFollowUpExtendedEditBu,

        [EnumMember(Value = "DW Quote follow-up Extended Edit (P/C)")]
        DwQuoteFollowUpExtendedEditPC,

        [EnumMember(Value = "DW Quote follow-up Extended Read (BU)")]
        DwQuoteFollowUpExtendedReadBu,

        [EnumMember(Value = "DW Quote follow-up Extended Read (P/C)")]
        DwQuoteFollowUpExtendedReadPC,

        [EnumMember(Value = "DW Quote follow-up Products Delete (P/C)")]
        DwQuoteFollowUpProductsDeletePC,

        [EnumMember(Value = "DW Report Wizard")]
        DwReportWizard,

        [EnumMember(Value = "DW SCP Manager")]
        DwScpManager,

        [EnumMember(Value = "DW SCP User Management")]
        DwScpUserManagement,

        [EnumMember(Value = "DW Service Order Read only (BU)")]
        DwServiceOrderReadOnlyBu,

        [EnumMember(Value = "DW Service Order Read only (P/C)")]
        DwServiceOrderReadOnlyPC,

        [EnumMember(Value = "DW SystemPrice User")]
        DwSystemPriceUser,

        [EnumMember(Value = "DW Team Extended Edit (BU)")]
        DwTeamExtendedEditBu,

        [EnumMember(Value = "DW Team Extended Edit (P/C)")]
        DwTeamExtendedEditPC,

        [EnumMember(Value = "DW_FORM_Installed Base_Installed Base Sales")]
        DwFormInstalledBaseInstalledBaseSales,

        [EnumMember(Value = "DW_FORM_Installed Base_Installed Base Service")]
        DwFormInstalledBaseInstalledBaseService,

        [EnumMember(Value = "Dynamics 365 App for Outlook User")]
        Dynamics365AppForOutlookUser,

        [EnumMember(Value = "Environment Maker")]
        EnvironmentMaker,

        [EnumMember(Value = "Export Customizations")]
        ExportCustomizations,

        [EnumMember(Value = "Forecast manager")]
        ForecastManager,

        [EnumMember(Value = "Forecast user")]
        ForecastUser,

        [EnumMember(Value = "Knowledge Manager")]
        KnowledgeManager,

        [EnumMember(Value = "Playbook Manager")]
        PlaybookManager,

        [EnumMember(Value = "Playbook User")]
        PlaybookUser,

        [EnumMember(Value = "Relationship Insights Admin")]
        RelationshipInsightsAdmin,

        [EnumMember(Value = "Solution Checker")]
        SolutionChecker,

        [EnumMember(Value = "Survey Owner")]
        SurveyOwner,

        [EnumMember(Value = "Survey Services Administrator")]
        SurveyServicesAdministrator,

        [EnumMember(Value = "System Administrator")]
        SystemAdministrator,

        [EnumMember(Value = "zz_CEO-Business Manager")]
        ZzCeoBusinessManager,

        [EnumMember(Value = "zz_CSR Manager")]
        ZzCsrManager,

        [EnumMember(Value = "zz_Customer Service Representative")]
        ZzCustomerServiceRepresentative,

        [EnumMember(Value = "zz_Delegate")]
        ZzDelegate,

        [EnumMember(Value = "zz_DW Basic CRM Access (initial draft version)")]
        ZzDwBasicCrmAccessInitialDraftVersion,

        [EnumMember(Value = "zz_Logging Readers")]
        ZzLoggingReaders,

        [EnumMember(Value = "zz_Marketing Manager")]
        ZzMarketingManager,

        [EnumMember(Value = "zz_Marketing Professional")]
        ZzMarketingProfessional,

        [EnumMember(Value = "zz_Sales Manager")]
        ZzSalesManager,

        [EnumMember(Value = "zz_Salesperson")]
        ZzSalesperson,

        [EnumMember(Value = "zz_Schedule Manager")]
        ZzScheduleManager,

        [EnumMember(Value = "zz_Scheduler")]
        ZzScheduler,

        [EnumMember(Value = "zz_System Customizer")]
        ZzSystemCustomizer,

        [EnumMember(Value = "zz_Vice President of Marketing")]
        ZzVicePresidentOfMarketing,

        [EnumMember(Value = "zz_Vice President of Sales")]
        ZzVicePresidentOfSales
    }
}