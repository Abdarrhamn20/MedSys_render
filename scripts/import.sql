--
-- PostgreSQL database dump
--


-- Dumped from database version 18.4
-- Dumped by pg_dump version 18.4

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

ALTER TABLE IF EXISTS ONLY public."WebPushSubscriptions" DROP CONSTRAINT IF EXISTS "FK_WebPushSubscriptions_Users_UserID";
ALTER TABLE IF EXISTS ONLY public."Vouchers" DROP CONSTRAINT IF EXISTS "FK_Vouchers_Users_PostedByUserID";
ALTER TABLE IF EXISTS ONLY public."Vouchers" DROP CONSTRAINT IF EXISTS "FK_Vouchers_Users_PatientUserID";
ALTER TABLE IF EXISTS ONLY public."Vouchers" DROP CONSTRAINT IF EXISTS "FK_Vouchers_Users_CreatedByUserID";
ALTER TABLE IF EXISTS ONLY public."Vouchers" DROP CONSTRAINT IF EXISTS "FK_Vouchers_Treasuries_TreasuryID";
ALTER TABLE IF EXISTS ONLY public."Vouchers" DROP CONSTRAINT IF EXISTS "FK_Vouchers_Treasuries_ToTreasuryID";
ALTER TABLE IF EXISTS ONLY public."Vouchers" DROP CONSTRAINT IF EXISTS "FK_Vouchers_Invoices_InvoiceID";
ALTER TABLE IF EXISTS ONLY public."Vouchers" DROP CONSTRAINT IF EXISTS "FK_Vouchers_ChartAccounts_AccountID";
ALTER TABLE IF EXISTS ONLY public."Vouchers" DROP CONSTRAINT IF EXISTS "FK_Vouchers_Appointments_AppointmentID";
ALTER TABLE IF EXISTS ONLY public."Users" DROP CONSTRAINT IF EXISTS "FK_Users_Treasuries_AssignedTreasuryID";
ALTER TABLE IF EXISTS ONLY public."UserNotifications" DROP CONSTRAINT IF EXISTS "FK_UserNotifications_Users_UserID";
ALTER TABLE IF EXISTS ONLY public."Treasuries" DROP CONSTRAINT IF EXISTS "FK_Treasuries_ChartAccounts_AccountID";
ALTER TABLE IF EXISTS ONLY public."TelemedicineSessions" DROP CONSTRAINT IF EXISTS "FK_TelemedicineSessions_Appointments_AppointmentID";
ALTER TABLE IF EXISTS ONLY public."StockMovements" DROP CONSTRAINT IF EXISTS "FK_StockMovements_Warehouses_WarehouseID";
ALTER TABLE IF EXISTS ONLY public."StockMovements" DROP CONSTRAINT IF EXISTS "FK_StockMovements_Warehouses_ToWarehouseID";
ALTER TABLE IF EXISTS ONLY public."StockMovements" DROP CONSTRAINT IF EXISTS "FK_StockMovements_Users_PostedByUserID";
ALTER TABLE IF EXISTS ONLY public."StockMovements" DROP CONSTRAINT IF EXISTS "FK_StockMovements_Users_CreatedByUserID";
ALTER TABLE IF EXISTS ONLY public."StockMovementItems" DROP CONSTRAINT IF EXISTS "FK_StockMovementItems_StockMovements_MovementID";
ALTER TABLE IF EXISTS ONLY public."StockMovementItems" DROP CONSTRAINT IF EXISTS "FK_StockMovementItems_InventoryItems_ItemID";
ALTER TABLE IF EXISTS ONLY public."StockCounts" DROP CONSTRAINT IF EXISTS "FK_StockCounts_Warehouses_WarehouseID";
ALTER TABLE IF EXISTS ONLY public."StockCounts" DROP CONSTRAINT IF EXISTS "FK_StockCounts_Users_ReversedByUserID";
ALTER TABLE IF EXISTS ONLY public."StockCounts" DROP CONSTRAINT IF EXISTS "FK_StockCounts_Users_PostedByUserID";
ALTER TABLE IF EXISTS ONLY public."StockCounts" DROP CONSTRAINT IF EXISTS "FK_StockCounts_Users_CreatedByUserID";
ALTER TABLE IF EXISTS ONLY public."StockCountItems" DROP CONSTRAINT IF EXISTS "FK_StockCountItems_StockCounts_StockCountID";
ALTER TABLE IF EXISTS ONLY public."StockCountItems" DROP CONSTRAINT IF EXISTS "FK_StockCountItems_InventoryItems_ItemID";
ALTER TABLE IF EXISTS ONLY public."SoapNotes" DROP CONSTRAINT IF EXISTS "FK_SoapNotes_MedicalRecords_RecordID";
ALTER TABLE IF EXISTS ONLY public."SensitivityResults" DROP CONSTRAINT IF EXISTS "FK_SensitivityResults_CultureSensitivities_CultureSensitivityID";
ALTER TABLE IF EXISTS ONLY public."SalaryRecords" DROP CONSTRAINT IF EXISTS "FK_SalaryRecords_Users_CreatedByUserID";
ALTER TABLE IF EXISTS ONLY public."SalaryRecords" DROP CONSTRAINT IF EXISTS "FK_SalaryRecords_JournalEntries_JournalEntryID";
ALTER TABLE IF EXISTS ONLY public."SalaryRecords" DROP CONSTRAINT IF EXISTS "FK_SalaryRecords_EmployeeProfiles_EmployeeID";
ALTER TABLE IF EXISTS ONLY public."Rooms" DROP CONSTRAINT IF EXISTS "FK_Rooms_Wards_WardID";
ALTER TABLE IF EXISTS ONLY public."RadiologyOrders" DROP CONSTRAINT IF EXISTS "FK_RadiologyOrders_Users_RadiologistID";
ALTER TABLE IF EXISTS ONLY public."RadiologyOrders" DROP CONSTRAINT IF EXISTS "FK_RadiologyOrders_Users_PatientUserID";
ALTER TABLE IF EXISTS ONLY public."RadiologyOrders" DROP CONSTRAINT IF EXISTS "FK_RadiologyOrders_Users_DoctorID";
ALTER TABLE IF EXISTS ONLY public."PsychiatricRecords" DROP CONSTRAINT IF EXISTS "FK_PsychiatricRecords_MedicalRecords_RecordID";
ALTER TABLE IF EXISTS ONLY public."Prescriptions" DROP CONSTRAINT IF EXISTS "FK_Prescriptions_Medications_MedicationID";
ALTER TABLE IF EXISTS ONLY public."Prescriptions" DROP CONSTRAINT IF EXISTS "FK_Prescriptions_MedicalRecords_RecordID";
ALTER TABLE IF EXISTS ONLY public."PatientProfiles" DROP CONSTRAINT IF EXISTS "FK_PatientProfiles_Users_UserID";
ALTER TABLE IF EXISTS ONLY public."PatientAssessments" DROP CONSTRAINT IF EXISTS "FK_PatientAssessments_Users_PatientUserID";
ALTER TABLE IF EXISTS ONLY public."PatientAssessments" DROP CONSTRAINT IF EXISTS "FK_PatientAssessments_CustomAssessmentTemplates_TemplateID";
ALTER TABLE IF EXISTS ONLY public."MedicationRequests" DROP CONSTRAINT IF EXISTS "FK_MedicationRequests_Users_DoctorUserID";
ALTER TABLE IF EXISTS ONLY public."MedicalRecords" DROP CONSTRAINT IF EXISTS "FK_MedicalRecords_Appointments_AppID";
ALTER TABLE IF EXISTS ONLY public."LabTests" DROP CONSTRAINT IF EXISTS "FK_LabTests_LabTests_PanelID";
ALTER TABLE IF EXISTS ONLY public."LabTests" DROP CONSTRAINT IF EXISTS "FK_LabTests_LabDevices_DeviceID";
ALTER TABLE IF EXISTS ONLY public."LabReferenceRanges" DROP CONSTRAINT IF EXISTS "FK_LabReferenceRanges_LabTests_LabTestID";
ALTER TABLE IF EXISTS ONLY public."LabOrders" DROP CONSTRAINT IF EXISTS "FK_LabOrders_Users_PatientUserID";
ALTER TABLE IF EXISTS ONLY public."LabOrders" DROP CONSTRAINT IF EXISTS "FK_LabOrders_Users_DoctorID";
ALTER TABLE IF EXISTS ONLY public."LabOrders" DROP CONSTRAINT IF EXISTS "FK_LabOrders_LabTests_LabTestID";
ALTER TABLE IF EXISTS ONLY public."LabOrderItems" DROP CONSTRAINT IF EXISTS "FK_LabOrderItems_LabTests_LabTestID";
ALTER TABLE IF EXISTS ONLY public."LabOrderItems" DROP CONSTRAINT IF EXISTS "FK_LabOrderItems_LabOrders_LabOrderID";
ALTER TABLE IF EXISTS ONLY public."JournalEntryLines" DROP CONSTRAINT IF EXISTS "FK_JournalEntryLines_JournalEntries_JournalEntryID";
ALTER TABLE IF EXISTS ONLY public."JournalEntryLines" DROP CONSTRAINT IF EXISTS "FK_JournalEntryLines_ChartAccounts_AccountID";
ALTER TABLE IF EXISTS ONLY public."JournalEntries" DROP CONSTRAINT IF EXISTS "FK_JournalEntries_Users_PostedByUserID";
ALTER TABLE IF EXISTS ONLY public."JournalEntries" DROP CONSTRAINT IF EXISTS "FK_JournalEntries_Users_CreatedByUserID";
ALTER TABLE IF EXISTS ONLY public."Invoices" DROP CONSTRAINT IF EXISTS "FK_Invoices_Users_PatientUserID";
ALTER TABLE IF EXISTS ONLY public."Invoices" DROP CONSTRAINT IF EXISTS "FK_Invoices_Users_DoctorID";
ALTER TABLE IF EXISTS ONLY public."Invoices" DROP CONSTRAINT IF EXISTS "FK_Invoices_RadiologyOrders_RadiologyOrderID";
ALTER TABLE IF EXISTS ONLY public."Invoices" DROP CONSTRAINT IF EXISTS "FK_Invoices_LabOrders_LabOrderID";
ALTER TABLE IF EXISTS ONLY public."Invoices" DROP CONSTRAINT IF EXISTS "FK_Invoices_DoctorCommissions_DoctorCommissionID";
ALTER TABLE IF EXISTS ONLY public."Invoices" DROP CONSTRAINT IF EXISTS "FK_Invoices_DispenseRecords_DispenseRecordID";
ALTER TABLE IF EXISTS ONLY public."Invoices" DROP CONSTRAINT IF EXISTS "FK_Invoices_Appointments_AppointmentID";
ALTER TABLE IF EXISTS ONLY public."InventoryItems" DROP CONSTRAINT IF EXISTS "FK_InventoryItems_Medications_MedicationID";
ALTER TABLE IF EXISTS ONLY public."InventoryItems" DROP CONSTRAINT IF EXISTS "FK_InventoryItems_InventoryCategories_CategoryID";
ALTER TABLE IF EXISTS ONLY public."InventoryCategories" DROP CONSTRAINT IF EXISTS "FK_InventoryCategories_InventoryCategories_ParentCategoryID";
ALTER TABLE IF EXISTS ONLY public."InpatientDailyLogs" DROP CONSTRAINT IF EXISTS "FK_InpatientDailyLogs_Users_LoggedByUserID";
ALTER TABLE IF EXISTS ONLY public."InpatientDailyLogs" DROP CONSTRAINT IF EXISTS "FK_InpatientDailyLogs_Admissions_AdmissionID";
ALTER TABLE IF EXISTS ONLY public."InpatientCareOrders" DROP CONSTRAINT IF EXISTS "FK_InpatientCareOrders_Users_CreatedByUserID";
ALTER TABLE IF EXISTS ONLY public."InpatientCareOrders" DROP CONSTRAINT IF EXISTS "FK_InpatientCareOrders_HealthServices_HealthServiceID";
ALTER TABLE IF EXISTS ONLY public."InpatientCareOrders" DROP CONSTRAINT IF EXISTS "FK_InpatientCareOrders_Admissions_AdmissionID";
ALTER TABLE IF EXISTS ONLY public."InpatientCareExecutions" DROP CONSTRAINT IF EXISTS "FK_InpatientCareExecutions_Users_ExecutedByUserID";
ALTER TABLE IF EXISTS ONLY public."InpatientCareExecutions" DROP CONSTRAINT IF EXISTS "FK_InpatientCareExecutions_InpatientCareOrders_OrderID";
ALTER TABLE IF EXISTS ONLY public."EmployeeProfiles" DROP CONSTRAINT IF EXISTS "FK_EmployeeProfiles_Users_UserID";
ALTER TABLE IF EXISTS ONLY public."EmployeeLeaves" DROP CONSTRAINT IF EXISTS "FK_EmployeeLeaves_Users_ApprovedByUserID";
ALTER TABLE IF EXISTS ONLY public."EmployeeLeaves" DROP CONSTRAINT IF EXISTS "FK_EmployeeLeaves_EmployeeProfiles_EmployeeID";
ALTER TABLE IF EXISTS ONLY public."EmployeeCourses" DROP CONSTRAINT IF EXISTS "FK_EmployeeCourses_EmployeeProfiles_EmployeeID";
ALTER TABLE IF EXISTS ONLY public."DoctorProfiles" DROP CONSTRAINT IF EXISTS "FK_DoctorProfiles_Users_UserID";
ALTER TABLE IF EXISTS ONLY public."DoctorCommissions" DROP CONSTRAINT IF EXISTS "FK_DoctorCommissions_Users_DoctorID";
ALTER TABLE IF EXISTS ONLY public."DispenseRecords" DROP CONSTRAINT IF EXISTS "FK_DispenseRecords_Users_DispensedByUserID";
ALTER TABLE IF EXISTS ONLY public."DispenseRecords" DROP CONSTRAINT IF EXISTS "FK_DispenseRecords_Prescriptions_PrescriptionID";
ALTER TABLE IF EXISTS ONLY public."DispenseRecords" DROP CONSTRAINT IF EXISTS "FK_DispenseRecords_Medications_MedicationID";
ALTER TABLE IF EXISTS ONLY public."CustomAssessmentTemplates" DROP CONSTRAINT IF EXISTS "FK_CustomAssessmentTemplates_DoctorProfiles_DoctorID";
ALTER TABLE IF EXISTS ONLY public."CultureSensitivities" DROP CONSTRAINT IF EXISTS "FK_CultureSensitivities_LabOrderItems_LabOrderItemID";
ALTER TABLE IF EXISTS ONLY public."ChartAccounts" DROP CONSTRAINT IF EXISTS "FK_ChartAccounts_ChartAccounts_ParentAccountID";
ALTER TABLE IF EXISTS ONLY public."Beds" DROP CONSTRAINT IF EXISTS "FK_Beds_Rooms_RoomID";
ALTER TABLE IF EXISTS ONLY public."AuditLogs" DROP CONSTRAINT IF EXISTS "FK_AuditLogs_Users_UserID";
ALTER TABLE IF EXISTS ONLY public."Attachments" DROP CONSTRAINT IF EXISTS "FK_Attachments_PatientProfiles_PatientID";
ALTER TABLE IF EXISTS ONLY public."Attachments" DROP CONSTRAINT IF EXISTS "FK_Attachments_MedicalRecords_RecordID";
ALTER TABLE IF EXISTS ONLY public."Appointments" DROP CONSTRAINT IF EXISTS "FK_Appointments_Priorities_PriorityID";
ALTER TABLE IF EXISTS ONLY public."Appointments" DROP CONSTRAINT IF EXISTS "FK_Appointments_PatientProfiles_PatientID";
ALTER TABLE IF EXISTS ONLY public."Appointments" DROP CONSTRAINT IF EXISTS "FK_Appointments_DoctorProfiles_DoctorID";
ALTER TABLE IF EXISTS ONLY public."Admissions" DROP CONSTRAINT IF EXISTS "FK_Admissions_PatientProfiles_PatientID";
ALTER TABLE IF EXISTS ONLY public."Admissions" DROP CONSTRAINT IF EXISTS "FK_Admissions_DoctorProfiles_DoctorID";
ALTER TABLE IF EXISTS ONLY public."Admissions" DROP CONSTRAINT IF EXISTS "FK_Admissions_Beds_BedID";
DROP INDEX IF EXISTS public."IX_WebPushSubscriptions_UserID";
DROP INDEX IF EXISTS public."IX_WebPushSubscriptions_Endpoint";
DROP INDEX IF EXISTS public."IX_Warehouses_WarehouseCode";
DROP INDEX IF EXISTS public."IX_Vouchers_VoucherNumber";
DROP INDEX IF EXISTS public."IX_Vouchers_TreasuryID";
DROP INDEX IF EXISTS public."IX_Vouchers_ToTreasuryID";
DROP INDEX IF EXISTS public."IX_Vouchers_PostedByUserID";
DROP INDEX IF EXISTS public."IX_Vouchers_PatientUserID";
DROP INDEX IF EXISTS public."IX_Vouchers_InvoiceID";
DROP INDEX IF EXISTS public."IX_Vouchers_CreatedByUserID";
DROP INDEX IF EXISTS public."IX_Vouchers_AppointmentID";
DROP INDEX IF EXISTS public."IX_Vouchers_AccountID";
DROP INDEX IF EXISTS public."IX_Users_Email";
DROP INDEX IF EXISTS public."IX_Users_AssignedTreasuryID";
DROP INDEX IF EXISTS public."IX_UserNotifications_UserID_IsRead";
DROP INDEX IF EXISTS public."IX_Treasuries_TreasuryCode";
DROP INDEX IF EXISTS public."IX_Treasuries_AccountID";
DROP INDEX IF EXISTS public."IX_TelemedicineSessions_RoomCode";
DROP INDEX IF EXISTS public."IX_TelemedicineSessions_AppointmentID";
DROP INDEX IF EXISTS public."IX_StockMovements_WarehouseID";
DROP INDEX IF EXISTS public."IX_StockMovements_ToWarehouseID";
DROP INDEX IF EXISTS public."IX_StockMovements_PostedByUserID";
DROP INDEX IF EXISTS public."IX_StockMovements_MovementNumber";
DROP INDEX IF EXISTS public."IX_StockMovements_CreatedByUserID";
DROP INDEX IF EXISTS public."IX_StockMovementItems_MovementID";
DROP INDEX IF EXISTS public."IX_StockMovementItems_ItemID";
DROP INDEX IF EXISTS public."IX_StockCounts_WarehouseID";
DROP INDEX IF EXISTS public."IX_StockCounts_StockCountNumber";
DROP INDEX IF EXISTS public."IX_StockCounts_ReversedByUserID";
DROP INDEX IF EXISTS public."IX_StockCounts_PostedByUserID";
DROP INDEX IF EXISTS public."IX_StockCounts_CreatedByUserID";
DROP INDEX IF EXISTS public."IX_StockCountItems_StockCountID_ItemID";
DROP INDEX IF EXISTS public."IX_StockCountItems_ItemID";
DROP INDEX IF EXISTS public."IX_SoapNotes_RecordID";
DROP INDEX IF EXISTS public."IX_SensitivityResults_CultureSensitivityID";
DROP INDEX IF EXISTS public."IX_SalaryRecords_JournalEntryID";
DROP INDEX IF EXISTS public."IX_SalaryRecords_EmployeeID_PeriodYear_PeriodMonth";
DROP INDEX IF EXISTS public."IX_SalaryRecords_CreatedByUserID";
DROP INDEX IF EXISTS public."IX_Rooms_WardID";
DROP INDEX IF EXISTS public."IX_RadiologyOrders_RadiologistID";
DROP INDEX IF EXISTS public."IX_RadiologyOrders_PatientUserID";
DROP INDEX IF EXISTS public."IX_RadiologyOrders_DoctorID";
DROP INDEX IF EXISTS public."IX_Prescriptions_RecordID";
DROP INDEX IF EXISTS public."IX_Prescriptions_MedicationID";
DROP INDEX IF EXISTS public."IX_PatientProfiles_UserID";
DROP INDEX IF EXISTS public."IX_PatientProfiles_FileNumber";
DROP INDEX IF EXISTS public."IX_PatientAssessments_TemplateID";
DROP INDEX IF EXISTS public."IX_PatientAssessments_PatientUserID";
DROP INDEX IF EXISTS public."IX_MedicationRequests_DoctorUserID";
DROP INDEX IF EXISTS public."IX_MedicalRecords_AppID";
DROP INDEX IF EXISTS public."IX_LabTests_PanelID";
DROP INDEX IF EXISTS public."IX_LabTests_DeviceID";
DROP INDEX IF EXISTS public."IX_LabReferenceRanges_LabTestID";
DROP INDEX IF EXISTS public."IX_LabOrders_PatientUserID";
DROP INDEX IF EXISTS public."IX_LabOrders_LabTestID";
DROP INDEX IF EXISTS public."IX_LabOrders_DoctorID";
DROP INDEX IF EXISTS public."IX_LabOrderItems_LabTestID";
DROP INDEX IF EXISTS public."IX_LabOrderItems_LabOrderID_LabTestID";
DROP INDEX IF EXISTS public."IX_LabDevices_DeviceCode";
DROP INDEX IF EXISTS public."IX_JournalEntryLines_JournalEntryID";
DROP INDEX IF EXISTS public."IX_JournalEntryLines_AccountID";
DROP INDEX IF EXISTS public."IX_JournalEntries_PostedByUserID";
DROP INDEX IF EXISTS public."IX_JournalEntries_EntryNumber";
DROP INDEX IF EXISTS public."IX_JournalEntries_CreatedByUserID";
DROP INDEX IF EXISTS public."IX_Invoices_RadiologyOrderID";
DROP INDEX IF EXISTS public."IX_Invoices_PatientUserID";
DROP INDEX IF EXISTS public."IX_Invoices_LabOrderID";
DROP INDEX IF EXISTS public."IX_Invoices_DoctorID";
DROP INDEX IF EXISTS public."IX_Invoices_DoctorCommissionID";
DROP INDEX IF EXISTS public."IX_Invoices_DispenseRecordID";
DROP INDEX IF EXISTS public."IX_Invoices_AppointmentID";
DROP INDEX IF EXISTS public."IX_InventoryItems_MedicationID";
DROP INDEX IF EXISTS public."IX_InventoryItems_ItemCode";
DROP INDEX IF EXISTS public."IX_InventoryItems_CategoryID";
DROP INDEX IF EXISTS public."IX_InventoryCategories_ParentCategoryID";
DROP INDEX IF EXISTS public."IX_InpatientDailyLogs_LoggedByUserID";
DROP INDEX IF EXISTS public."IX_InpatientDailyLogs_AdmissionID";
DROP INDEX IF EXISTS public."IX_InpatientCareOrders_HealthServiceID";
DROP INDEX IF EXISTS public."IX_InpatientCareOrders_CreatedByUserID";
DROP INDEX IF EXISTS public."IX_InpatientCareOrders_AdmissionID";
DROP INDEX IF EXISTS public."IX_InpatientCareExecutions_OrderID";
DROP INDEX IF EXISTS public."IX_InpatientCareExecutions_ExecutedByUserID";
DROP INDEX IF EXISTS public."IX_EmployeeProfiles_UserID";
DROP INDEX IF EXISTS public."IX_EmployeeProfiles_EmployeeNumber";
DROP INDEX IF EXISTS public."IX_EmployeeLeaves_EmployeeID";
DROP INDEX IF EXISTS public."IX_EmployeeLeaves_ApprovedByUserID";
DROP INDEX IF EXISTS public."IX_EmployeeCourses_EmployeeID";
DROP INDEX IF EXISTS public."IX_DoctorProfiles_UserID";
DROP INDEX IF EXISTS public."IX_DoctorCommissions_DoctorID";
DROP INDEX IF EXISTS public."IX_DispenseRecords_PrescriptionID";
DROP INDEX IF EXISTS public."IX_DispenseRecords_MedicationID";
DROP INDEX IF EXISTS public."IX_DispenseRecords_DispensedByUserID";
DROP INDEX IF EXISTS public."IX_CustomAssessmentTemplates_DoctorID";
DROP INDEX IF EXISTS public."IX_CultureSensitivities_LabOrderItemID";
DROP INDEX IF EXISTS public."IX_ChartAccounts_ParentAccountID";
DROP INDEX IF EXISTS public."IX_ChartAccounts_AccountCode";
DROP INDEX IF EXISTS public."IX_Beds_RoomID";
DROP INDEX IF EXISTS public."IX_AuditLogs_UserID";
DROP INDEX IF EXISTS public."IX_Attachments_RecordID";
DROP INDEX IF EXISTS public."IX_Attachments_PatientID";
DROP INDEX IF EXISTS public."IX_Appointments_PriorityID";
DROP INDEX IF EXISTS public."IX_Appointments_PatientID";
DROP INDEX IF EXISTS public."IX_Appointments_DoctorID";
DROP INDEX IF EXISTS public."IX_Admissions_PatientID";
DROP INDEX IF EXISTS public."IX_Admissions_DoctorID";
DROP INDEX IF EXISTS public."IX_Admissions_BedID";
ALTER TABLE IF EXISTS ONLY public."__EFMigrationsHistory" DROP CONSTRAINT IF EXISTS "PK___EFMigrationsHistory";
ALTER TABLE IF EXISTS ONLY public."WebPushSubscriptions" DROP CONSTRAINT IF EXISTS "PK_WebPushSubscriptions";
ALTER TABLE IF EXISTS ONLY public."Warehouses" DROP CONSTRAINT IF EXISTS "PK_Warehouses";
ALTER TABLE IF EXISTS ONLY public."Wards" DROP CONSTRAINT IF EXISTS "PK_Wards";
ALTER TABLE IF EXISTS ONLY public."Vouchers" DROP CONSTRAINT IF EXISTS "PK_Vouchers";
ALTER TABLE IF EXISTS ONLY public."Users" DROP CONSTRAINT IF EXISTS "PK_Users";
ALTER TABLE IF EXISTS ONLY public."UserNotifications" DROP CONSTRAINT IF EXISTS "PK_UserNotifications";
ALTER TABLE IF EXISTS ONLY public."TriageQuestions" DROP CONSTRAINT IF EXISTS "PK_TriageQuestions";
ALTER TABLE IF EXISTS ONLY public."Treasuries" DROP CONSTRAINT IF EXISTS "PK_Treasuries";
ALTER TABLE IF EXISTS ONLY public."TelemedicineSessions" DROP CONSTRAINT IF EXISTS "PK_TelemedicineSessions";
ALTER TABLE IF EXISTS ONLY public."SystemSettings" DROP CONSTRAINT IF EXISTS "PK_SystemSettings";
ALTER TABLE IF EXISTS ONLY public."StockMovements" DROP CONSTRAINT IF EXISTS "PK_StockMovements";
ALTER TABLE IF EXISTS ONLY public."StockMovementItems" DROP CONSTRAINT IF EXISTS "PK_StockMovementItems";
ALTER TABLE IF EXISTS ONLY public."StockCounts" DROP CONSTRAINT IF EXISTS "PK_StockCounts";
ALTER TABLE IF EXISTS ONLY public."StockCountItems" DROP CONSTRAINT IF EXISTS "PK_StockCountItems";
ALTER TABLE IF EXISTS ONLY public."SoapNotes" DROP CONSTRAINT IF EXISTS "PK_SoapNotes";
ALTER TABLE IF EXISTS ONLY public."SensitivityResults" DROP CONSTRAINT IF EXISTS "PK_SensitivityResults";
ALTER TABLE IF EXISTS ONLY public."SalaryRecords" DROP CONSTRAINT IF EXISTS "PK_SalaryRecords";
ALTER TABLE IF EXISTS ONLY public."Rooms" DROP CONSTRAINT IF EXISTS "PK_Rooms";
ALTER TABLE IF EXISTS ONLY public."RadiologyTemplates" DROP CONSTRAINT IF EXISTS "PK_RadiologyTemplates";
ALTER TABLE IF EXISTS ONLY public."RadiologyOrders" DROP CONSTRAINT IF EXISTS "PK_RadiologyOrders";
ALTER TABLE IF EXISTS ONLY public."PsychiatricRecords" DROP CONSTRAINT IF EXISTS "PK_PsychiatricRecords";
ALTER TABLE IF EXISTS ONLY public."Priorities" DROP CONSTRAINT IF EXISTS "PK_Priorities";
ALTER TABLE IF EXISTS ONLY public."Prescriptions" DROP CONSTRAINT IF EXISTS "PK_Prescriptions";
ALTER TABLE IF EXISTS ONLY public."PatientProfiles" DROP CONSTRAINT IF EXISTS "PK_PatientProfiles";
ALTER TABLE IF EXISTS ONLY public."PatientAssessments" DROP CONSTRAINT IF EXISTS "PK_PatientAssessments";
ALTER TABLE IF EXISTS ONLY public."Medications" DROP CONSTRAINT IF EXISTS "PK_Medications";
ALTER TABLE IF EXISTS ONLY public."MedicationRequests" DROP CONSTRAINT IF EXISTS "PK_MedicationRequests";
ALTER TABLE IF EXISTS ONLY public."MedicalRecords" DROP CONSTRAINT IF EXISTS "PK_MedicalRecords";
ALTER TABLE IF EXISTS ONLY public."LabTests" DROP CONSTRAINT IF EXISTS "PK_LabTests";
ALTER TABLE IF EXISTS ONLY public."LabReferenceRanges" DROP CONSTRAINT IF EXISTS "PK_LabReferenceRanges";
ALTER TABLE IF EXISTS ONLY public."LabOrders" DROP CONSTRAINT IF EXISTS "PK_LabOrders";
ALTER TABLE IF EXISTS ONLY public."LabOrderItems" DROP CONSTRAINT IF EXISTS "PK_LabOrderItems";
ALTER TABLE IF EXISTS ONLY public."LabDevices" DROP CONSTRAINT IF EXISTS "PK_LabDevices";
ALTER TABLE IF EXISTS ONLY public."JournalEntryLines" DROP CONSTRAINT IF EXISTS "PK_JournalEntryLines";
ALTER TABLE IF EXISTS ONLY public."JournalEntries" DROP CONSTRAINT IF EXISTS "PK_JournalEntries";
ALTER TABLE IF EXISTS ONLY public."Invoices" DROP CONSTRAINT IF EXISTS "PK_Invoices";
ALTER TABLE IF EXISTS ONLY public."InventoryItems" DROP CONSTRAINT IF EXISTS "PK_InventoryItems";
ALTER TABLE IF EXISTS ONLY public."InventoryCategories" DROP CONSTRAINT IF EXISTS "PK_InventoryCategories";
ALTER TABLE IF EXISTS ONLY public."InpatientDailyLogs" DROP CONSTRAINT IF EXISTS "PK_InpatientDailyLogs";
ALTER TABLE IF EXISTS ONLY public."InpatientCareOrders" DROP CONSTRAINT IF EXISTS "PK_InpatientCareOrders";
ALTER TABLE IF EXISTS ONLY public."InpatientCareExecutions" DROP CONSTRAINT IF EXISTS "PK_InpatientCareExecutions";
ALTER TABLE IF EXISTS ONLY public."HealthServices" DROP CONSTRAINT IF EXISTS "PK_HealthServices";
ALTER TABLE IF EXISTS ONLY public."EmployeeProfiles" DROP CONSTRAINT IF EXISTS "PK_EmployeeProfiles";
ALTER TABLE IF EXISTS ONLY public."EmployeeLeaves" DROP CONSTRAINT IF EXISTS "PK_EmployeeLeaves";
ALTER TABLE IF EXISTS ONLY public."EmployeeCourses" DROP CONSTRAINT IF EXISTS "PK_EmployeeCourses";
ALTER TABLE IF EXISTS ONLY public."DoctorProfiles" DROP CONSTRAINT IF EXISTS "PK_DoctorProfiles";
ALTER TABLE IF EXISTS ONLY public."DoctorCommissions" DROP CONSTRAINT IF EXISTS "PK_DoctorCommissions";
ALTER TABLE IF EXISTS ONLY public."DispenseRecords" DROP CONSTRAINT IF EXISTS "PK_DispenseRecords";
ALTER TABLE IF EXISTS ONLY public."CustomAssessmentTemplates" DROP CONSTRAINT IF EXISTS "PK_CustomAssessmentTemplates";
ALTER TABLE IF EXISTS ONLY public."CultureSensitivities" DROP CONSTRAINT IF EXISTS "PK_CultureSensitivities";
ALTER TABLE IF EXISTS ONLY public."ChartAccounts" DROP CONSTRAINT IF EXISTS "PK_ChartAccounts";
ALTER TABLE IF EXISTS ONLY public."Beds" DROP CONSTRAINT IF EXISTS "PK_Beds";
ALTER TABLE IF EXISTS ONLY public."AuditLogs" DROP CONSTRAINT IF EXISTS "PK_AuditLogs";
ALTER TABLE IF EXISTS ONLY public."Attachments" DROP CONSTRAINT IF EXISTS "PK_Attachments";
ALTER TABLE IF EXISTS ONLY public."Appointments" DROP CONSTRAINT IF EXISTS "PK_Appointments";
ALTER TABLE IF EXISTS ONLY public."Admissions" DROP CONSTRAINT IF EXISTS "PK_Admissions";
DROP TABLE IF EXISTS public."__EFMigrationsHistory";
DROP TABLE IF EXISTS public."WebPushSubscriptions";
DROP TABLE IF EXISTS public."Warehouses";
DROP TABLE IF EXISTS public."Wards";
DROP TABLE IF EXISTS public."Vouchers";
DROP TABLE IF EXISTS public."Users";
DROP TABLE IF EXISTS public."UserNotifications";
DROP TABLE IF EXISTS public."TriageQuestions";
DROP TABLE IF EXISTS public."Treasuries";
DROP TABLE IF EXISTS public."TelemedicineSessions";
DROP TABLE IF EXISTS public."SystemSettings";
DROP TABLE IF EXISTS public."StockMovements";
DROP TABLE IF EXISTS public."StockMovementItems";
DROP TABLE IF EXISTS public."StockCounts";
DROP TABLE IF EXISTS public."StockCountItems";
DROP TABLE IF EXISTS public."SoapNotes";
DROP TABLE IF EXISTS public."SensitivityResults";
DROP TABLE IF EXISTS public."SalaryRecords";
DROP TABLE IF EXISTS public."Rooms";
DROP TABLE IF EXISTS public."RadiologyTemplates";
DROP TABLE IF EXISTS public."RadiologyOrders";
DROP TABLE IF EXISTS public."PsychiatricRecords";
DROP TABLE IF EXISTS public."Priorities";
DROP TABLE IF EXISTS public."Prescriptions";
DROP TABLE IF EXISTS public."PatientProfiles";
DROP TABLE IF EXISTS public."PatientAssessments";
DROP TABLE IF EXISTS public."Medications";
DROP TABLE IF EXISTS public."MedicationRequests";
DROP TABLE IF EXISTS public."MedicalRecords";
DROP TABLE IF EXISTS public."LabTests";
DROP TABLE IF EXISTS public."LabReferenceRanges";
DROP TABLE IF EXISTS public."LabOrders";
DROP TABLE IF EXISTS public."LabOrderItems";
DROP TABLE IF EXISTS public."LabDevices";
DROP TABLE IF EXISTS public."JournalEntryLines";
DROP TABLE IF EXISTS public."JournalEntries";
DROP TABLE IF EXISTS public."Invoices";
DROP TABLE IF EXISTS public."InventoryItems";
DROP TABLE IF EXISTS public."InventoryCategories";
DROP TABLE IF EXISTS public."InpatientDailyLogs";
DROP TABLE IF EXISTS public."InpatientCareOrders";
DROP TABLE IF EXISTS public."InpatientCareExecutions";
DROP TABLE IF EXISTS public."HealthServices";
DROP TABLE IF EXISTS public."EmployeeProfiles";
DROP TABLE IF EXISTS public."EmployeeLeaves";
DROP TABLE IF EXISTS public."EmployeeCourses";
DROP TABLE IF EXISTS public."DoctorProfiles";
DROP TABLE IF EXISTS public."DoctorCommissions";
DROP TABLE IF EXISTS public."DispenseRecords";
DROP TABLE IF EXISTS public."CustomAssessmentTemplates";
DROP TABLE IF EXISTS public."CultureSensitivities";
DROP TABLE IF EXISTS public."ChartAccounts";
DROP TABLE IF EXISTS public."Beds";
DROP TABLE IF EXISTS public."AuditLogs";
DROP TABLE IF EXISTS public."Attachments";
DROP TABLE IF EXISTS public."Appointments";
DROP TABLE IF EXISTS public."Admissions";
SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Admissions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Admissions" (
    "AdmissionID" integer NOT NULL,
    "PatientID" integer NOT NULL,
    "DoctorID" integer NOT NULL,
    "BedID" integer NOT NULL,
    "AdmissionDate" timestamp without time zone NOT NULL,
    "DischargeDate" timestamp without time zone,
    "AdmissionReason" character varying(500) NOT NULL,
    "Status" character varying(20) NOT NULL,
    "DischargeSummary" text,
    "CreatedAt" timestamp without time zone NOT NULL,
    "RowVersion" bytea
);


--
-- Name: Admissions_AdmissionID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."Admissions" ALTER COLUMN "AdmissionID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Admissions_AdmissionID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Appointments; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Appointments" (
    "AppID" integer NOT NULL,
    "PatientID" integer NOT NULL,
    "DoctorID" integer NOT NULL,
    "PriorityID" integer NOT NULL,
    "AppointmentDate" timestamp without time zone NOT NULL,
    "AppointmentTime" interval NOT NULL,
    "Status" character varying(20) DEFAULT 'Pending'::character varying NOT NULL,
    "TriageScore" integer NOT NULL,
    "Notes" character varying(500),
    "AppointmentType" character varying(20) NOT NULL,
    "QueueNumber" integer NOT NULL,
    "PaymentMethod" character varying(30),
    "CancellationReason" character varying(500),
    "CreatedAt" timestamp without time zone NOT NULL,
    "RowVersion" bytea NOT NULL
);


--
-- Name: Appointments_AppID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."Appointments" ALTER COLUMN "AppID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Appointments_AppID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Attachments; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Attachments" (
    "AttachmentID" integer NOT NULL,
    "RecordID" integer,
    "PatientID" integer,
    "FileName" character varying(200) NOT NULL,
    "FileType" character varying(50) NOT NULL,
    "FileURL" character varying(500) NOT NULL,
    "FileSize" bigint NOT NULL,
    "Description" character varying(300),
    "UploadedAt" timestamp without time zone NOT NULL
);


--
-- Name: Attachments_AttachmentID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."Attachments" ALTER COLUMN "AttachmentID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Attachments_AttachmentID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: AuditLogs; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."AuditLogs" (
    "LogID" integer NOT NULL,
    "ActionType" character varying(50) NOT NULL,
    "EntityType" character varying(100) NOT NULL,
    "EntityID" integer NOT NULL,
    "UserID" integer NOT NULL,
    "Details" character varying(500) NOT NULL,
    "Timestamp" timestamp without time zone NOT NULL
);


--
-- Name: AuditLogs_LogID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."AuditLogs" ALTER COLUMN "LogID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."AuditLogs_LogID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Beds; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Beds" (
    "BedID" integer NOT NULL,
    "RoomID" integer NOT NULL,
    "BedNumber" character varying(20) NOT NULL,
    "Status" character varying(20) NOT NULL,
    "Notes" character varying(255)
);


--
-- Name: Beds_BedID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."Beds" ALTER COLUMN "BedID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Beds_BedID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: ChartAccounts; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ChartAccounts" (
    "AccountID" integer NOT NULL,
    "AccountCode" character varying(20) NOT NULL,
    "AccountName" character varying(100) NOT NULL,
    "AccountNameAr" character varying(100) NOT NULL,
    "AccountType" character varying(20) DEFAULT 'Asset'::character varying NOT NULL,
    "ParentAccountID" integer,
    "OpeningBalance" numeric(18,2) NOT NULL,
    "IsActive" boolean DEFAULT true NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: ChartAccounts_AccountID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."ChartAccounts" ALTER COLUMN "AccountID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."ChartAccounts_AccountID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: CultureSensitivities; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."CultureSensitivities" (
    "CultureSensitivityID" integer NOT NULL,
    "LabOrderItemID" integer NOT NULL,
    "Organism" character varying(200),
    "GramStain" character varying(20),
    "CultureStatus" character varying(20) NOT NULL,
    "QuantitativeResult" character varying(50),
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: CultureSensitivities_CultureSensitivityID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."CultureSensitivities" ALTER COLUMN "CultureSensitivityID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."CultureSensitivities_CultureSensitivityID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: CustomAssessmentTemplates; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."CustomAssessmentTemplates" (
    "TemplateID" integer NOT NULL,
    "DoctorID" integer,
    "Title" character varying(150) NOT NULL,
    "Description" character varying(500),
    "SchemaJson" text NOT NULL,
    "TemplateType" character varying(20) NOT NULL,
    "IsStandard" boolean NOT NULL,
    "MaxScore" integer,
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: CustomAssessmentTemplates_TemplateID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."CustomAssessmentTemplates" ALTER COLUMN "TemplateID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."CustomAssessmentTemplates_TemplateID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: DispenseRecords; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."DispenseRecords" (
    "DispenseID" integer NOT NULL,
    "PrescriptionID" integer NOT NULL,
    "MedicationID" integer,
    "QuantityDispensed" integer NOT NULL,
    "TotalPrice" numeric(18,2) NOT NULL,
    "DispensedByUserID" integer NOT NULL,
    "Status" character varying(30) NOT NULL,
    "Notes" character varying(300),
    "DispensedAt" timestamp without time zone NOT NULL
);


--
-- Name: DispenseRecords_DispenseID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."DispenseRecords" ALTER COLUMN "DispenseID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."DispenseRecords_DispenseID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: DoctorCommissions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."DoctorCommissions" (
    "CommissionID" integer NOT NULL,
    "DoctorID" integer NOT NULL,
    "Specialty" character varying(100),
    "ServiceID" integer,
    "CommissionType" character varying(20) NOT NULL,
    "Value" numeric(18,2) NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: DoctorCommissions_CommissionID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."DoctorCommissions" ALTER COLUMN "CommissionID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."DoctorCommissions_CommissionID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: DoctorProfiles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."DoctorProfiles" (
    "DoctorID" integer NOT NULL,
    "UserID" integer NOT NULL,
    "Specialty" character varying(100) NOT NULL,
    "LicenseNumber" character varying(50),
    "EmergencyReady" boolean NOT NULL,
    "Bio" character varying(500),
    "ImageUrl" character varying(300),
    "AvailableDays" character varying(100),
    "WorkStartTime" interval,
    "WorkEndTime" interval,
    "ConsultationDurationMinutes" integer NOT NULL,
    "ConsultationFee" numeric(18,2) NOT NULL
);


--
-- Name: DoctorProfiles_DoctorID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."DoctorProfiles" ALTER COLUMN "DoctorID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."DoctorProfiles_DoctorID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: EmployeeCourses; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."EmployeeCourses" (
    "CourseID" integer NOT NULL,
    "EmployeeID" integer NOT NULL,
    "CourseName" character varying(150) NOT NULL,
    "Provider" character varying(100),
    "CourseDate" timestamp without time zone NOT NULL,
    "CertificateNumber" character varying(50),
    "ExpiryDate" timestamp without time zone,
    "Notes" character varying(300),
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: EmployeeCourses_CourseID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."EmployeeCourses" ALTER COLUMN "CourseID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."EmployeeCourses_CourseID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: EmployeeLeaves; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."EmployeeLeaves" (
    "LeaveID" integer NOT NULL,
    "EmployeeID" integer NOT NULL,
    "LeaveType" character varying(20) NOT NULL,
    "StartDate" timestamp without time zone NOT NULL,
    "EndDate" timestamp without time zone NOT NULL,
    "Days" integer NOT NULL,
    "Reason" character varying(300),
    "Status" character varying(20) DEFAULT 'Pending'::character varying NOT NULL,
    "ApprovedByUserID" integer,
    "ApprovedAt" timestamp without time zone,
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: EmployeeLeaves_LeaveID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."EmployeeLeaves" ALTER COLUMN "LeaveID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."EmployeeLeaves_LeaveID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: EmployeeProfiles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."EmployeeProfiles" (
    "EmployeeID" integer NOT NULL,
    "UserID" integer,
    "EmployeeNumber" character varying(30) NOT NULL,
    "FullName" character varying(100) NOT NULL,
    "Department" character varying(100),
    "Position" character varying(100),
    "HireDate" timestamp without time zone NOT NULL,
    "Gender" character varying(20),
    "NationalID" character varying(30),
    "CompensationModel" character varying(20) DEFAULT 'FixedSalary'::character varying NOT NULL,
    "BaseSalary" numeric(18,2) DEFAULT 0.0 NOT NULL,
    "BankAccount" character varying(50),
    "IsActive" boolean DEFAULT true NOT NULL,
    "Notes" character varying(500),
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: EmployeeProfiles_EmployeeID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."EmployeeProfiles" ALTER COLUMN "EmployeeID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."EmployeeProfiles_EmployeeID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: HealthServices; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."HealthServices" (
    "ServiceID" integer NOT NULL,
    "ServiceName" character varying(200) NOT NULL,
    "ServiceNameAr" character varying(200) NOT NULL,
    "Category" character varying(100),
    "Description" character varying(500),
    "Price" numeric(18,2) NOT NULL,
    "Unit" character varying(50),
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: HealthServices_ServiceID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."HealthServices" ALTER COLUMN "ServiceID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."HealthServices_ServiceID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: InpatientCareExecutions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."InpatientCareExecutions" (
    "ExecutionID" integer NOT NULL,
    "OrderID" integer NOT NULL,
    "ExecutedByUserID" integer NOT NULL,
    "ExecutedAt" timestamp without time zone NOT NULL,
    "Status" character varying(20) NOT NULL,
    "Notes" text,
    "VitalTemperature" character varying(20),
    "VitalBloodPressure" character varying(20),
    "VitalPulse" character varying(20),
    "VitalOxygen" character varying(20)
);


--
-- Name: InpatientCareExecutions_ExecutionID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."InpatientCareExecutions" ALTER COLUMN "ExecutionID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."InpatientCareExecutions_ExecutionID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: InpatientCareOrders; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."InpatientCareOrders" (
    "OrderID" integer NOT NULL,
    "AdmissionID" integer NOT NULL,
    "HealthServiceID" integer,
    "OrderType" character varying(30) NOT NULL,
    "OrderDescription" character varying(255) NOT NULL,
    "Frequency" character varying(30) NOT NULL,
    "ScheduledTime" timestamp without time zone NOT NULL,
    "UnitPrice" numeric(18,2) NOT NULL,
    "Status" character varying(20) NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "CreatedByUserID" integer NOT NULL
);


--
-- Name: InpatientCareOrders_OrderID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."InpatientCareOrders" ALTER COLUMN "OrderID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."InpatientCareOrders_OrderID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: InpatientDailyLogs; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."InpatientDailyLogs" (
    "LogID" integer NOT NULL,
    "AdmissionID" integer NOT NULL,
    "LoggedByUserID" integer NOT NULL,
    "LogDate" timestamp without time zone NOT NULL,
    "Temperature" character varying(20),
    "BloodPressure" character varying(20),
    "PulseRate" character varying(20),
    "OxygenLevel" character varying(20),
    "DoctorNotes" text,
    "NursingNotes" text
);


--
-- Name: InpatientDailyLogs_LogID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."InpatientDailyLogs" ALTER COLUMN "LogID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."InpatientDailyLogs_LogID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: InventoryCategories; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."InventoryCategories" (
    "CategoryID" integer NOT NULL,
    "CategoryName" character varying(100) NOT NULL,
    "CategoryNameAr" character varying(100) NOT NULL,
    "ParentCategoryID" integer,
    "IsActive" boolean DEFAULT true NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: InventoryCategories_CategoryID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."InventoryCategories" ALTER COLUMN "CategoryID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."InventoryCategories_CategoryID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: InventoryItems; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."InventoryItems" (
    "ItemID" integer NOT NULL,
    "ItemCode" character varying(50) NOT NULL,
    "ItemName" character varying(200) NOT NULL,
    "ItemNameAr" character varying(200) NOT NULL,
    "CategoryID" integer NOT NULL,
    "MedicationID" integer,
    "Unit" character varying(50) NOT NULL,
    "PurchasePrice" numeric(18,2) NOT NULL,
    "SellingPrice" numeric(18,2) NOT NULL,
    "ReorderLevel" integer NOT NULL,
    "Manufacturer" character varying(200),
    "ExpiryDate" timestamp without time zone,
    "IsActive" boolean DEFAULT true NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: InventoryItems_ItemID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."InventoryItems" ALTER COLUMN "ItemID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."InventoryItems_ItemID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Invoices; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Invoices" (
    "InvoiceID" integer NOT NULL,
    "PatientUserID" integer NOT NULL,
    "AppointmentID" integer,
    "DispenseRecordID" integer,
    "LabOrderID" integer,
    "RadiologyOrderID" integer,
    "InvoiceType" character varying(50) NOT NULL,
    "Amount" numeric(18,2) NOT NULL,
    "Tax" numeric(18,2) NOT NULL,
    "Discount" numeric(18,2) NOT NULL,
    "TotalAmount" numeric(18,2) NOT NULL,
    "Status" character varying(30) NOT NULL,
    "PaymentMethod" character varying(30),
    "TransactionReference" character varying(100),
    "CreatedAt" timestamp without time zone NOT NULL,
    "PaidAt" timestamp without time zone,
    "DoctorShare" numeric(18,2) NOT NULL,
    "ClinicShare" numeric(18,2) NOT NULL,
    "DoctorID" integer,
    "DoctorCommissionID" integer
);


--
-- Name: Invoices_InvoiceID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."Invoices" ALTER COLUMN "InvoiceID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Invoices_InvoiceID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: JournalEntries; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."JournalEntries" (
    "JournalEntryID" integer NOT NULL,
    "EntryNumber" character varying(30) NOT NULL,
    "EntryDate" timestamp without time zone NOT NULL,
    "Description" character varying(200) NOT NULL,
    "SourceModule" character varying(30),
    "SourceReferenceID" integer,
    "Status" character varying(20) DEFAULT 'Draft'::character varying NOT NULL,
    "CreatedByUserID" integer NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "PostedAt" timestamp without time zone,
    "PostedByUserID" integer
);


--
-- Name: JournalEntries_JournalEntryID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."JournalEntries" ALTER COLUMN "JournalEntryID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."JournalEntries_JournalEntryID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: JournalEntryLines; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."JournalEntryLines" (
    "JournalEntryLineID" integer NOT NULL,
    "JournalEntryID" integer NOT NULL,
    "AccountID" integer NOT NULL,
    "Debit" numeric(18,2) NOT NULL,
    "Credit" numeric(18,2) NOT NULL,
    "Notes" character varying(200)
);


--
-- Name: JournalEntryLines_JournalEntryLineID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."JournalEntryLines" ALTER COLUMN "JournalEntryLineID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."JournalEntryLines_JournalEntryLineID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: LabDevices; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."LabDevices" (
    "LabDeviceID" integer NOT NULL,
    "DeviceName" character varying(100) NOT NULL,
    "DeviceCode" character varying(50) NOT NULL,
    "DeviceModel" character varying(100),
    "ConnectionType" character varying(30) NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: LabDevices_LabDeviceID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."LabDevices" ALTER COLUMN "LabDeviceID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."LabDevices_LabDeviceID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: LabOrderItems; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."LabOrderItems" (
    "LabOrderItemID" integer NOT NULL,
    "LabOrderID" integer NOT NULL,
    "LabTestID" integer NOT NULL,
    "ResultValue" character varying(500),
    "ResultStatus" character varying(20) NOT NULL,
    "TechnicianNotes" character varying(500),
    "CompletedAt" timestamp without time zone
);


--
-- Name: LabOrderItems_LabOrderItemID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."LabOrderItems" ALTER COLUMN "LabOrderItemID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."LabOrderItems_LabOrderItemID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: LabOrders; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."LabOrders" (
    "LabOrderID" integer NOT NULL,
    "PatientUserID" integer NOT NULL,
    "DoctorID" integer NOT NULL,
    "LabTestID" integer NOT NULL,
    "ResultValue" character varying(50),
    "ResultStatus" character varying(20) NOT NULL,
    "Status" character varying(30) NOT NULL,
    "ResultNotes" character varying(500),
    "TechnicianNotes" character varying(500),
    "RequestedAt" timestamp without time zone NOT NULL,
    "CompletedAt" timestamp without time zone,
    "VerificationQRCode" character varying(200)
);


--
-- Name: LabOrders_LabOrderID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."LabOrders" ALTER COLUMN "LabOrderID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."LabOrders_LabOrderID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: LabReferenceRanges; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."LabReferenceRanges" (
    "RangeID" integer NOT NULL,
    "LabTestID" integer NOT NULL,
    "Gender" character varying(10) NOT NULL,
    "MinAge" integer NOT NULL,
    "MaxAge" integer NOT NULL,
    "NormalMin" numeric(18,2) NOT NULL,
    "NormalMax" numeric(18,2) NOT NULL,
    "RangeNotes" character varying(50)
);


--
-- Name: LabReferenceRanges_RangeID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."LabReferenceRanges" ALTER COLUMN "RangeID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."LabReferenceRanges_RangeID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: LabTests; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."LabTests" (
    "LabTestID" integer NOT NULL,
    "TestName" character varying(150) NOT NULL,
    "Code" character varying(50) NOT NULL,
    "Category" character varying(100) NOT NULL,
    "Price" numeric(18,2) NOT NULL,
    "Unit" character varying(50) NOT NULL,
    "IsPanel" boolean NOT NULL,
    "PanelID" integer,
    "DeviceID" integer,
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: LabTests_LabTestID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."LabTests" ALTER COLUMN "LabTestID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."LabTests_LabTestID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: MedicalRecords; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."MedicalRecords" (
    "RecordID" integer NOT NULL,
    "AppID" integer NOT NULL,
    "Diagnosis" character varying(1000) NOT NULL,
    "DiagnosisAr" character varying(1000),
    "TreatmentPlan" character varying(2000),
    "DoctorNotes" character varying(2000),
    "Symptoms" character varying(500),
    "Recommendations" character varying(500),
    "RequiresFollowUp" boolean NOT NULL,
    "FollowUpDate" timestamp without time zone,
    "FollowUpNotes" character varying(500),
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: MedicalRecords_RecordID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."MedicalRecords" ALTER COLUMN "RecordID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."MedicalRecords_RecordID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: MedicationRequests; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."MedicationRequests" (
    "RequestID" integer NOT NULL,
    "MedicationName" character varying(200) NOT NULL,
    "DoctorUserID" integer NOT NULL,
    "DoctorName" character varying(200) NOT NULL,
    "Notes" character varying(500),
    "IsResolved" boolean NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: MedicationRequests_RequestID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."MedicationRequests" ALTER COLUMN "RequestID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."MedicationRequests_RequestID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Medications; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Medications" (
    "MedicationID" integer NOT NULL,
    "Name" character varying(200) NOT NULL,
    "NameAr" character varying(200) NOT NULL,
    "Category" character varying(100),
    "DosageForm" character varying(100),
    "Unit" character varying(50),
    "QuantityInStock" integer NOT NULL,
    "MinStockLevel" integer NOT NULL,
    "PurchasePrice" numeric(18,2) NOT NULL,
    "SellingPrice" numeric(18,2) NOT NULL,
    "Manufacturer" character varying(200),
    "ExpiryDate" timestamp without time zone,
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: Medications_MedicationID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."Medications" ALTER COLUMN "MedicationID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Medications_MedicationID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: PatientAssessments; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."PatientAssessments" (
    "AssessmentID" integer NOT NULL,
    "PatientUserID" integer NOT NULL,
    "TemplateID" integer NOT NULL,
    "AnswersJson" text NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "CompletedAt" timestamp without time zone,
    "Status" character varying(30) NOT NULL
);


--
-- Name: PatientAssessments_AssessmentID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."PatientAssessments" ALTER COLUMN "AssessmentID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."PatientAssessments_AssessmentID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: PatientProfiles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."PatientProfiles" (
    "PatientID" integer NOT NULL,
    "UserID" integer NOT NULL,
    "FirstName" character varying(100),
    "FatherName" character varying(100),
    "GrandfatherName" character varying(100),
    "FamilyName" character varying(100),
    "FileNumber" character varying(20),
    "MergedIntoPatientID" integer,
    "MergedAt" timestamp without time zone,
    "BloodType" character varying(5),
    "ChronicDiseases" character varying(500),
    "Allergies" character varying(500),
    "GeneralNotes" character varying(500),
    "DateOfBirth" timestamp without time zone,
    "Gender" character varying(10),
    "Address" character varying(200),
    "EmergencyContact" character varying(100),
    "EmergencyPhone" character varying(20),
    "RiskLevel" character varying(20) DEFAULT 'Stable'::character varying,
    "RiskLevelUpdatedAt" timestamp without time zone,
    "RiskLevelUpdatedByUserID" integer,
    "RiskLevelNotes" character varying(500)
);


--
-- Name: PatientProfiles_PatientID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."PatientProfiles" ALTER COLUMN "PatientID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."PatientProfiles_PatientID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Prescriptions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Prescriptions" (
    "PrescriptionID" integer NOT NULL,
    "RecordID" integer NOT NULL,
    "MedicationID" integer,
    "MedicationName" character varying(200) NOT NULL,
    "Dosage" character varying(100) NOT NULL,
    "Duration" character varying(100),
    "Instructions" character varying(300),
    "Frequency" character varying(50),
    "Quantity" integer NOT NULL,
    "DispenseStatus" character varying(30) NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: Prescriptions_PrescriptionID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."Prescriptions" ALTER COLUMN "PrescriptionID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Prescriptions_PrescriptionID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Priorities; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Priorities" (
    "PriorityID" integer NOT NULL,
    "LevelName" character varying(30) NOT NULL,
    "LevelNameAr" character varying(30) NOT NULL,
    "Weight" integer NOT NULL,
    "ColorCode" character varying(10) NOT NULL,
    "Icon" character varying(30) NOT NULL
);


--
-- Name: Priorities_PriorityID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."Priorities" ALTER COLUMN "PriorityID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Priorities_PriorityID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: PsychiatricRecords; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."PsychiatricRecords" (
    "RecordID" integer NOT NULL,
    "Appearance" character varying(1000),
    "Behavior" character varying(1000),
    "Speech" character varying(1000),
    "MoodAndAffect" character varying(1000),
    "ThoughtProcess" character varying(1000),
    "ThoughtContent" character varying(1000),
    "Perception" character varying(1000),
    "Cognition" character varying(1000),
    "InsightAndJudgment" character varying(1000),
    "IsSpeechToTextUsed" boolean NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: RadiologyOrders; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."RadiologyOrders" (
    "RadiologyOrderID" integer NOT NULL,
    "PatientUserID" integer NOT NULL,
    "DoctorID" integer NOT NULL,
    "Modality" character varying(50) NOT NULL,
    "BodyPart" character varying(100) NOT NULL,
    "Status" character varying(30) NOT NULL,
    "ReportText" text,
    "ImagePath" character varying(500),
    "RequestedAt" timestamp without time zone NOT NULL,
    "CompletedAt" timestamp without time zone,
    "RadiologistID" integer
);


--
-- Name: RadiologyOrders_RadiologyOrderID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."RadiologyOrders" ALTER COLUMN "RadiologyOrderID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."RadiologyOrders_RadiologyOrderID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: RadiologyTemplates; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."RadiologyTemplates" (
    "TemplateID" integer NOT NULL,
    "TemplateName" character varying(100) NOT NULL,
    "Modality" character varying(50) NOT NULL,
    "BodyPart" character varying(100) NOT NULL,
    "DefaultReportText" text NOT NULL,
    "Price" numeric(18,2) NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: RadiologyTemplates_TemplateID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."RadiologyTemplates" ALTER COLUMN "TemplateID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."RadiologyTemplates_TemplateID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Rooms; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Rooms" (
    "RoomID" integer NOT NULL,
    "WardID" integer NOT NULL,
    "RoomNumber" character varying(50) NOT NULL,
    "RoomType" character varying(30) NOT NULL,
    "DailyRate" numeric(18,2) NOT NULL,
    "MaxBeds" integer NOT NULL,
    "IsActive" boolean NOT NULL
);


--
-- Name: Rooms_RoomID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."Rooms" ALTER COLUMN "RoomID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Rooms_RoomID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: SalaryRecords; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."SalaryRecords" (
    "SalaryRecordID" integer NOT NULL,
    "EmployeeID" integer NOT NULL,
    "PeriodYear" integer NOT NULL,
    "PeriodMonth" integer NOT NULL,
    "BaseSalary" numeric(18,2) NOT NULL,
    "CommissionAmount" numeric(18,2) NOT NULL,
    "Bonus" numeric(18,2) NOT NULL,
    "Deduction" numeric(18,2) NOT NULL,
    "GrossSalary" numeric(18,2) NOT NULL,
    "NetSalary" numeric(18,2) NOT NULL,
    "Status" character varying(20) DEFAULT 'Draft'::character varying NOT NULL,
    "JournalEntryID" integer,
    "CreatedByUserID" integer NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "PostedAt" timestamp without time zone
);


--
-- Name: SalaryRecords_SalaryRecordID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."SalaryRecords" ALTER COLUMN "SalaryRecordID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."SalaryRecords_SalaryRecordID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: SensitivityResults; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."SensitivityResults" (
    "SensitivityResultID" integer NOT NULL,
    "CultureSensitivityID" integer NOT NULL,
    "AntibioticName" character varying(100) NOT NULL,
    "Interpretation" character varying(20) NOT NULL,
    "ZoneDiameter" numeric(18,2)
);


--
-- Name: SensitivityResults_SensitivityResultID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."SensitivityResults" ALTER COLUMN "SensitivityResultID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."SensitivityResults_SensitivityResultID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: SoapNotes; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."SoapNotes" (
    "SoapNoteID" integer NOT NULL,
    "RecordID" integer NOT NULL,
    "Subjective" text,
    "Objective" text,
    "Assessment" text,
    "Plan" text,
    "CreatedAt" timestamp without time zone NOT NULL,
    "UpdatedAt" timestamp without time zone
);


--
-- Name: SoapNotes_SoapNoteID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."SoapNotes" ALTER COLUMN "SoapNoteID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."SoapNotes_SoapNoteID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: StockCountItems; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."StockCountItems" (
    "StockCountItemID" integer NOT NULL,
    "StockCountID" integer NOT NULL,
    "ItemID" integer NOT NULL,
    "SystemQuantity" numeric(18,2) NOT NULL,
    "CountedQuantity" numeric(18,2) NOT NULL,
    "UnitPrice" numeric(18,2) NOT NULL,
    "Notes" character varying(200)
);


--
-- Name: StockCountItems_StockCountItemID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."StockCountItems" ALTER COLUMN "StockCountItemID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."StockCountItems_StockCountItemID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: StockCounts; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."StockCounts" (
    "StockCountID" integer NOT NULL,
    "StockCountNumber" character varying(30) NOT NULL,
    "CountDate" timestamp without time zone NOT NULL,
    "WarehouseID" integer NOT NULL,
    "Status" character varying(20) DEFAULT 'Draft'::character varying NOT NULL,
    "Notes" character varying(300) NOT NULL,
    "CreatedByUserID" integer NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "PostedByUserID" integer,
    "PostedAt" timestamp without time zone,
    "ReversedByUserID" integer,
    "ReversedAt" timestamp without time zone
);


--
-- Name: StockCounts_StockCountID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."StockCounts" ALTER COLUMN "StockCountID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."StockCounts_StockCountID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: StockMovementItems; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."StockMovementItems" (
    "StockMovementItemID" integer NOT NULL,
    "MovementID" integer NOT NULL,
    "ItemID" integer NOT NULL,
    "Quantity" numeric(18,2) NOT NULL,
    "UnitPrice" numeric(18,2) NOT NULL,
    "Notes" character varying(200)
);


--
-- Name: StockMovementItems_StockMovementItemID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."StockMovementItems" ALTER COLUMN "StockMovementItemID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."StockMovementItems_StockMovementItemID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: StockMovements; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."StockMovements" (
    "MovementID" integer NOT NULL,
    "MovementNumber" character varying(30) NOT NULL,
    "MovementType" character varying(20) NOT NULL,
    "MovementDate" timestamp without time zone NOT NULL,
    "WarehouseID" integer NOT NULL,
    "ToWarehouseID" integer,
    "ReferenceType" character varying(100) NOT NULL,
    "ReferenceID" integer,
    "Notes" character varying(300) NOT NULL,
    "Status" character varying(20) DEFAULT 'Draft'::character varying NOT NULL,
    "CreatedByUserID" integer NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "PostedByUserID" integer,
    "PostedAt" timestamp without time zone
);


--
-- Name: StockMovements_MovementID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."StockMovements" ALTER COLUMN "MovementID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."StockMovements_MovementID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: SystemSettings; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."SystemSettings" (
    "SettingKey" character varying(100) NOT NULL,
    "SettingValue" character varying(255) NOT NULL,
    "UpdatedAt" timestamp without time zone NOT NULL
);


--
-- Name: TelemedicineSessions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."TelemedicineSessions" (
    "SessionID" integer NOT NULL,
    "AppointmentID" integer NOT NULL,
    "RoomCode" character varying(36) NOT NULL,
    "Status" character varying(20) NOT NULL,
    "CreatedByUserID" integer NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "StartedAt" timestamp without time zone,
    "EndedAt" timestamp without time zone,
    "SessionNotes" character varying(500)
);


--
-- Name: TelemedicineSessions_SessionID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."TelemedicineSessions" ALTER COLUMN "SessionID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."TelemedicineSessions_SessionID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Treasuries; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Treasuries" (
    "TreasuryID" integer NOT NULL,
    "TreasuryName" character varying(50) NOT NULL,
    "TreasuryNameAr" character varying(50) NOT NULL,
    "TreasuryCode" character varying(20) NOT NULL,
    "AccountID" integer NOT NULL,
    "IsActive" boolean DEFAULT true NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: Treasuries_TreasuryID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."Treasuries" ALTER COLUMN "TreasuryID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Treasuries_TreasuryID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: TriageQuestions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."TriageQuestions" (
    "QuestionID" integer NOT NULL,
    "QuestionText" character varying(300) NOT NULL,
    "QuestionTextAr" character varying(300) NOT NULL,
    "Weight" integer NOT NULL,
    "Category" character varying(50) NOT NULL,
    "IsActive" boolean NOT NULL,
    "SortOrder" integer NOT NULL
);


--
-- Name: TriageQuestions_QuestionID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."TriageQuestions" ALTER COLUMN "QuestionID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."TriageQuestions_QuestionID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: UserNotifications; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."UserNotifications" (
    "NotificationID" integer NOT NULL,
    "UserID" integer NOT NULL,
    "Title" character varying(150) NOT NULL,
    "Message" character varying(500),
    "Type" character varying(50) NOT NULL,
    "RelatedEntityType" character varying(50),
    "RelatedEntityID" integer,
    "IsRead" boolean NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: UserNotifications_NotificationID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."UserNotifications" ALTER COLUMN "NotificationID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."UserNotifications_NotificationID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Users; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Users" (
    "UserID" integer NOT NULL,
    "FullName" character varying(100) NOT NULL,
    "Email" character varying(150) NOT NULL,
    "Password" text NOT NULL,
    "Role" character varying(30) DEFAULT 'Patient'::character varying NOT NULL,
    "Phone" character varying(20),
    "AssignedTreasuryID" integer,
    "IsActive" boolean DEFAULT true NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: Users_UserID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."Users" ALTER COLUMN "UserID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Users_UserID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Vouchers; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Vouchers" (
    "VoucherID" integer NOT NULL,
    "VoucherNumber" character varying(30) NOT NULL,
    "VoucherType" character varying(20) NOT NULL,
    "VoucherDate" timestamp without time zone NOT NULL,
    "TreasuryID" integer NOT NULL,
    "ToTreasuryID" integer,
    "AccountID" integer,
    "PatientUserID" integer,
    "InvoiceID" integer,
    "AppointmentID" integer,
    "Amount" numeric(18,2) NOT NULL,
    "Description" character varying(200) NOT NULL,
    "Status" character varying(20) DEFAULT 'Draft'::character varying NOT NULL,
    "CreatedByUserID" integer NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "PostedByUserID" integer,
    "PostedAt" timestamp without time zone
);


--
-- Name: Vouchers_VoucherID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."Vouchers" ALTER COLUMN "VoucherID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Vouchers_VoucherID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Wards; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Wards" (
    "WardID" integer NOT NULL,
    "WardName" character varying(100) NOT NULL,
    "WardNameAr" character varying(100) NOT NULL,
    "GenderType" character varying(20) NOT NULL,
    "FloorNumber" integer NOT NULL,
    "IsActive" boolean NOT NULL
);


--
-- Name: Wards_WardID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."Wards" ALTER COLUMN "WardID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Wards_WardID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Warehouses; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Warehouses" (
    "WarehouseID" integer NOT NULL,
    "WarehouseName" character varying(50) NOT NULL,
    "WarehouseNameAr" character varying(50) NOT NULL,
    "WarehouseCode" character varying(20) NOT NULL,
    "Location" character varying(200),
    "IsActive" boolean DEFAULT true NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL
);


--
-- Name: Warehouses_WarehouseID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."Warehouses" ALTER COLUMN "WarehouseID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Warehouses_WarehouseID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: WebPushSubscriptions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."WebPushSubscriptions" (
    "SubscriptionID" integer NOT NULL,
    "UserID" integer NOT NULL,
    "Endpoint" character varying(500) NOT NULL,
    "P256DH" character varying(256) NOT NULL,
    "Auth" character varying(128) NOT NULL,
    "UserAgent" character varying(255),
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp without time zone NOT NULL,
    "LastUsedAt" timestamp without time zone
);


--
-- Name: WebPushSubscriptions_SubscriptionID_seq; Type: SEQUENCE; Schema: public; Owner: -
--

ALTER TABLE public."WebPushSubscriptions" ALTER COLUMN "SubscriptionID" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."WebPushSubscriptions_SubscriptionID_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


--
-- Data for Name: Admissions; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."Admissions" ("AdmissionID", "PatientID", "DoctorID", "BedID", "AdmissionDate", "DischargeDate", "AdmissionReason", "Status", "DischargeSummary", "CreatedAt", "RowVersion") FROM stdin;
1	4	5	1	2026-07-24 13:18:17.213169	\N	لديه نزيف في الانف	Active	\N	2026-07-24 13:18:17.212519	\\x000000000002b751
2	6	2	2	2026-07-24 13:27:10.438429	\N	لديه التهاب	Active	\N	2026-07-24 13:27:10.437816	\\x000000000002d691
3	10	7	3	2026-08-06 20:32:47.281059	2026-08-06 20:32:47.984833	اختبار تنويم	Discharged	تحسنت الحالة	2026-08-06 18:32:47.280844	\\x0000000000044d92
\.


--
-- Data for Name: Appointments; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."Appointments" ("AppID", "PatientID", "DoctorID", "PriorityID", "AppointmentDate", "AppointmentTime", "Status", "TriageScore", "Notes", "AppointmentType", "QueueNumber", "PaymentMethod", "CancellationReason", "CreatedAt", "RowVersion") FROM stdin;
1	1	1	3	2026-05-19 00:00:00	12:00:00	Completed	70			0	\N	\N	2026-05-05 00:00:46.330969	\\x00000000000084d1
2	2	1	3	2026-05-19 00:00:00	13:00:00	Completed	70			0	\N	\N	2026-05-06 15:59:45.702282	\\x00000000000084d4
3	3	1	2	2026-05-22 00:00:00	11:00:00	Completed	40			0	\N	\N	2026-05-21 19:31:57.264705	\\x0000000000012113
4	1	1	3	2026-05-26 00:00:00	12:00:00	Completed	70			0	\N	\N	2026-05-22 14:13:00.946487	\\x000000000001dc92
5	3	2	2	2026-05-30 00:00:00	12:30:00	Confirmed	40			0	\N	\N	2026-05-22 15:12:17.309457	\\x0000000000015f91
6	3	1	3	2026-05-24 00:00:00	12:00:00	Confirmed	70			0	\N	\N	2026-05-23 15:32:53.77172	\\x0000000000012114
7	1	1	2	2026-05-26 00:00:00	10:00:00	Confirmed	35			0	\N	\N	2026-05-24 19:11:42.123793	\\x0000000000031511
8	2	3	3	2026-07-14 00:00:00	12:00:00	Completed	50			0	\N	\N	2026-07-11 02:05:52.434969	\\x000000000001fbd3
9	4	3	2	2026-07-14 00:00:00	11:00:00	Completed	35			0	\N	\N	2026-07-11 03:59:14.966309	\\x0000000000021b14
10	5	3	3	2026-07-22 00:00:00	12:00:00	Completed	50			0	\N	\N	2026-07-14 11:29:31.667043	\\x0000000000025993
11	5	4	2	2026-07-21 00:00:00	13:00:00	Pending	30			0	\N	\N	2026-07-14 11:41:53.446093	\\x0000000000025994
12	6	5	2	2026-07-23 00:00:00	13:00:00	Confirmed	30			0	\N	\N	2026-07-15 14:42:12.845307	\\x00000000000278d2
13	7	3	1	2026-07-31 00:00:00	14:38:12.617858	Confirmed	0		WalkIn	1	Cash	\N	2026-07-31 14:38:12.618541	\\x000000000002f5d1
14	1	5	1	2026-08-04 00:00:00	20:54:31.076326	Completed	0		WalkIn	1	Cash	\N	2026-08-04 20:54:31.077493	\\x000000000003b154
15	4	5	2	2026-08-04 00:00:00	11:20:00	Completed	30		WalkIn	1	Cash	\N	2026-08-04 21:47:35.633034	\\x000000000003b152
16	9	5	1	2026-08-04 00:00:00	22:55:27.834996	Confirmed	0		WalkIn	3	Cash	\N	2026-08-04 22:55:27.835512	\\x00000000000372d1
17	9	1	1	2026-08-04 00:00:00	23:18:56.356581	Confirmed	0		WalkIn	1	Cash	\N	2026-08-04 23:18:56.356909	\\x0000000000039211
18	5	1	2	2026-08-09 00:00:00	12:30:00	Completed	33	نزلة برد	Online	1	Cash	\N	2026-08-06 17:56:50.130632	\\x000000000003efd2
19	10	7	1	2026-08-07 00:00:00	12:30:00	Completed	0	\N	Online	1	Cash	\N	2026-08-06 19:38:19.312349	\\x0000000000046cd1
20	12	7	1	2026-08-06 00:00:00	20:45:05.149889	Confirmed	0	حجز سريع Walk-in	WalkIn	1	Cash	\N	2026-08-06 20:45:05.150028	\\x0000000000048c11
21	4	1	2	2026-08-30 00:00:00	11:55:00	Pending	45		Online	1	Cash	\N	2026-08-27 18:12:35.664793	\\x00000000000566d1
\.


--
-- Data for Name: Attachments; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."Attachments" ("AttachmentID", "RecordID", "PatientID", "FileName", "FileType", "FileURL", "FileSize", "Description", "UploadedAt") FROM stdin;
1	2	\N	‎⁨تاريخ المريض الطبي اون لاين⁩.pdf	application/pdf	/uploads/attachments/a7448295-6287-4945-96ee-75052b167dea.pdf	1100913	\N	2026-05-20 16:03:07.222009
\.


--
-- Data for Name: AuditLogs; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."AuditLogs" ("LogID", "ActionType", "EntityType", "EntityID", "UserID", "Details", "Timestamp") FROM stdin;
1	StatusChange	Appointment	2	3	تم تغيير حالة الموعد من Confirmed إلى InProgress	2026-05-20 16:04:14.335681
2	StatusChange	Appointment	2	3	تم تغيير حالة الموعد من InProgress إلى Completed	2026-05-20 16:04:42.282096
3	AppointmentCreated	Appointment	3	6	تم حجز موعد جديد مع الطبيب طه احمد	2026-05-21 19:31:57.339653
4	StatusChange	Appointment	3	3	تم تغيير حالة الموعد من Pending إلى Confirmed	2026-05-21 19:32:46.068749
5	AppointmentCreated	Appointment	4	2	تم حجز موعد جديد مع الطبيب طه احمد	2026-05-22 14:13:01.249433
6	StatusChange	Appointment	4	3	تم تغيير حالة الموعد من Confirmed إلى InProgress	2026-05-22 14:18:44.111695
7	StatusChange	Appointment	4	3	تم تغيير حالة الموعد من InProgress إلى Completed	2026-05-22 14:18:48.860375
8	MedicationRequestCreated	MedicationRequest	1	3	طلب الطبيب د. طه احمد توفير دواء: فوار	2026-05-22 14:28:01.465959
9	MedicationRequestCreated	MedicationRequest	2	3	طلب الطبيب د. طه احمد توفير دواء: امبرازول	2026-05-22 14:47:07.523956
10	AppointmentCreated	Appointment	5	6	تم حجز موعد جديد مع الطبيب محمد علي	2026-05-22 15:12:17.811225
11	StatusChange	Appointment	3	3	تم تغيير حالة الموعد من Confirmed إلى InProgress	2026-05-23 15:30:02.986324
12	StatusChange	Appointment	3	3	تم تغيير حالة الموعد من InProgress إلى Completed	2026-05-23 15:30:17.82095
13	AppointmentCreated	Appointment	6	6	تم حجز موعد جديد مع الطبيب طه احمد	2026-05-23 15:32:53.885921
14	StatusChange	Appointment	5	7	تم تغيير حالة الموعد من Pending إلى Confirmed	2026-05-23 15:34:45.076619
15	StatusChange	Appointment	5	7	تم تغيير حالة الموعد من Confirmed إلى InProgress	2026-05-23 15:34:54.244166
16	StatusChange	Appointment	5	7	تم تغيير حالة الموعد من InProgress إلى Completed	2026-05-23 15:34:57.327321
17	AppointmentCreated	Appointment	7	2	تم حجز موعد جديد مع الطبيب طه احمد	2026-05-24 19:11:42.662912
20	MedicationRequestResolved	MedicationRequest	2	1	تم حل طلب توفير الدواء: امبرازول بنجاح	2026-06-30 15:15:44.980691
21	MedicationRequestResolved	MedicationRequest	1	1	تم حل طلب توفير الدواء: فوار بنجاح	2026-06-30 15:15:56.636643
22	StatusChange	Appointment	4	3	تم تغيير حالة الموعد من Confirmed إلى InProgress	2026-07-09 18:47:22.803158
23	StatusChange	Appointment	4	3	تم تغيير حالة الموعد من InProgress إلى Completed	2026-07-09 18:47:28.170898
24	PrescriptionDispensed	Prescription	3	5	تم صرف الوصفة: باراسيتامول (paracetamol) - الكمية: 1	2026-07-09 18:53:05.230144
26	AppointmentCreated	Appointment	8	4	تم حجز موعد جديد مع الطبيب محمد مراد	2026-07-11 02:05:52.705363
27	StatusChange	Appointment	8	8	تم تغيير حالة الموعد من Confirmed إلى InProgress	2026-07-11 02:07:00.460738
28	StatusChange	Appointment	8	8	تم تغيير حالة الموعد من InProgress إلى Completed	2026-07-11 02:07:02.180867
29	PrescriptionDispensed	Prescription	4	5	تم صرف الوصفة: بنادول (panadol) - الكمية: 1	2026-07-11 03:32:25.629461
30	CreatePsychiatricRecord	PsychiatricRecord	5	8	إنشاء سجل فحص الحالة العقلية (MSE) التابع للسجل الطبي #5	2026-07-11 03:35:21.779913
31	AppointmentCreated	Appointment	9	9	تم حجز موعد جديد مع الطبيب محمد مراد	2026-07-11 03:59:15.130925
32	StatusChange	Appointment	9	8	تم تغيير حالة الموعد من Pending إلى Confirmed	2026-07-11 04:00:09.139746
33	StatusChange	Appointment	9	8	تم تغيير حالة الموعد من Confirmed إلى InProgress	2026-07-11 04:00:12.201805
34	StatusChange	Appointment	9	8	تم تغيير حالة الموعد من InProgress إلى Completed	2026-07-11 04:00:13.395914
35	CreatePsychiatricRecord	PsychiatricRecord	6	8	إنشاء سجل فحص الحالة العقلية (MSE) التابع للسجل الطبي #6	2026-07-11 04:03:13.021105
36	CreateSoapNote	SoapNote	6	8	إنشاء سجل SOAP Note للسجل الطبي #6	2026-07-11 04:04:19.014101
37	UpdateRiskLevel	PatientProfile	4	8	تحديث مستوى الخطورة للمريض اسامه إلى: تحت الملاحظة 🟡 — ملاحظات: وضعه متدهور	2026-07-11 04:04:52.185729
38	AppointmentCreated	Appointment	10	10	تم حجز موعد جديد مع الطبيب محمد مراد	2026-07-14 11:29:31.827065
39	StatusChange	Appointment	10	8	تم تغيير حالة الموعد من Confirmed إلى InProgress	2026-07-14 11:33:07.827126
40	StatusChange	Appointment	10	8	تم تغيير حالة الموعد من InProgress إلى Completed	2026-07-14 11:33:20.954171
41	MedicationRequestCreated	MedicationRequest	3	8	طلب الطبيب د. محمد مراد توفير دواء: فولتارين	2026-07-14 11:35:15.781554
42	AppointmentCreated	Appointment	11	10	تم حجز موعد جديد مع الطبيب عبد الله	2026-07-14 11:41:53.45985
43	AppointmentCreated	Appointment	12	12	تم حجز موعد جديد مع الطبيب hamza	2026-07-15 14:42:12.998474
44	StatusChange	Appointment	12	13	تم تغيير حالة الموعد من Pending إلى Confirmed	2026-07-15 14:42:42.562878
45	StatusChange	Appointment	7	3	تم تغيير حالة الموعد من Pending إلى Confirmed	2026-08-03 19:29:26.630919
46	AppointmentCreated	Appointment	15	9	تم حجز موعد جديد مع الطبيب hamza (درجة الفرز: 30)	2026-08-04 21:47:36.121888
47	StatusChange	Appointment	15	13	تم تغيير حالة الموعد من Pending إلى Confirmed	2026-08-04 21:51:21.832243
49	StatusChange	Appointment	15	13	تم تغيير حالة الموعد من Confirmed إلى InProgress	2026-08-06 17:09:18.372677
50	StatusChange	Appointment	15	13	تم تغيير حالة الموعد من InProgress إلى Completed	2026-08-06 17:09:26.887322
51	StatusChange	Appointment	14	13	تم تغيير حالة الموعد من Confirmed إلى InProgress	2026-08-06 17:09:35.866825
52	StatusChange	Appointment	14	13	تم تغيير حالة الموعد من InProgress إلى Completed	2026-08-06 17:09:38.236532
53	AppointmentCreated	Appointment	18	10	تم حجز موعد جديد مع الطبيب طه احمد (درجة الفرز: 33)	2026-08-06 17:56:50.6219
54	StatusChange	Appointment	18	3	تم تغيير حالة الموعد من Pending إلى Confirmed	2026-08-06 17:57:22.661342
55	TelemedicineStarted	Appointment	18	3	تم بدء جلسة فيديو عن بعد للموعد #18	2026-08-06 18:02:50.103229
56	TelemedicineEnded	TelemedicineSession	1	3	انتهت جلسة الفيديو للموعد #18	2026-08-06 18:03:49.023115
57	TelemedicineEnded	TelemedicineSession	2	3	انتهت جلسة الفيديو للموعد #18	2026-08-06 18:04:13.637091
58	StatusChange	Appointment	18	3	تم تغيير حالة الموعد من InProgress إلى Completed	2026-08-06 18:04:23.434248
59	AppointmentCreated	Appointment	19	22	تم حجز موعد جديد مع الطبيب Test Doctor (درجة الفرز: 0)	2026-08-06 19:38:19.542041
61	PrescriptionDispensed	Prescription	6	26	تم صرف الوصفة: بنادول (panadol) - الكمية: 1	2026-08-06 19:49:32.463771
62	TelemedicineStarted	Appointment	19	23	تم بدء جلسة فيديو عن بعد للموعد #19	2026-08-06 20:29:18.386325
63	TelemedicineEnded	TelemedicineSession	3	22	انتهت جلسة الفيديو للموعد #19	2026-08-06 20:29:18.752305
64	JournalEntryCreated	JournalEntry	0	1	إنشاء قيد JE-2026-0001 بمبلغ 100.00 د.ل — Test entry: cash deposit	2026-08-07 19:22:53.520976
65	JournalEntryPosted	JournalEntry	1	1	ترحيل قيد JE-2026-0001 بقيمة 100.00 د.ل	2026-08-07 19:22:53.820973
66	JournalEntryCreated	JournalEntry	0	1	إنشاء قيد JE-2026-0001 بمبلغ 100.00 د.ل — Test entry: cash deposit	2026-08-07 19:24:36.973874
67	JournalEntryPosted	JournalEntry	2	1	ترحيل قيد JE-2026-0001 بقيمة 100.00 د.ل	2026-08-07 19:24:37.256872
81	WarehouseCreated	Warehouse	0	1	إنشاء مخزن warehouse_2_ar بكود WARE-02	2026-08-07 19:50:38.129502
82	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف medicines_ar	2026-08-07 19:50:38.342603
83	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف analgesics_ar	2026-08-07 19:50:38.391719
84	ItemCreated	InventoryItem	0	1	إنشاء صنف paracetamol_ar بكود ITM-001	2026-08-07 19:50:38.49637
85	ItemCreated	InventoryItem	0	1	إنشاء صنف bandage_ar بكود ITM-002	2026-08-07 19:50:38.532745
86	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0001 (In)	2026-08-07 19:50:38.742667
87	MovementPosted	StockMovement	1	1	ترحيل سند مخزن MV-2026-0001 (In)	2026-08-07 19:50:38.875947
88	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0002 (Out)	2026-08-07 19:50:38.923607
89	MovementPosted	StockMovement	2	1	ترحيل سند مخزن MV-2026-0002 (Out)	2026-08-07 19:50:38.95274
90	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0003 (Out)	2026-08-07 19:50:39.016713
91	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0004 (Out)	2026-08-07 19:50:39.030565
92	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0005 (Transfer)	2026-08-07 19:50:39.066206
93	MovementPosted	StockMovement	5	1	ترحيل سند مخزن MV-2026-0005 (Transfer)	2026-08-07 19:50:39.08064
94	MovementReversed	StockMovement	5	1	عكس سند مخزن MV-2026-0005 (Transfer)	2026-08-07 19:50:39.257408
95	ItemUpdated	InventoryItem	1	1	تعديل صنف paracetamol_ar_v2	2026-08-07 19:50:39.348687
96	CategoryUpdated	InventoryCategory	2	1	تعديل فئة analgesics_ar_v2	2026-08-07 19:50:39.369255
97	WarehouseUpdated	Warehouse	2	1	تعديل مخزن warehouse_2_ar_v2	2026-08-07 19:50:39.392628
98	WarehouseCreated	Warehouse	0	1	إنشاء مخزن warehouse_2_ar بكود WARE-02	2026-08-07 19:55:19.315018
99	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف medicines_ar	2026-08-07 19:55:19.517641
100	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف analgesics_ar	2026-08-07 19:55:19.567824
101	ItemCreated	InventoryItem	0	1	إنشاء صنف paracetamol_ar بكود ITM-001	2026-08-07 19:55:19.697466
102	ItemCreated	InventoryItem	0	1	إنشاء صنف bandage_ar بكود ITM-002	2026-08-07 19:55:19.7395
103	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0001 (In)	2026-08-07 19:55:19.903601
104	MovementPosted	StockMovement	1	1	ترحيل سند مخزن MV-2026-0001 (In)	2026-08-07 19:55:20.040666
105	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0002 (Out)	2026-08-07 19:55:20.092191
106	MovementPosted	StockMovement	2	1	ترحيل سند مخزن MV-2026-0002 (Out)	2026-08-07 19:55:20.124907
107	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0003 (Out)	2026-08-07 19:55:20.157531
108	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0004 (Transfer)	2026-08-07 19:55:20.190499
109	MovementPosted	StockMovement	4	1	ترحيل سند مخزن MV-2026-0004 (Transfer)	2026-08-07 19:55:20.20764
110	MovementReversed	StockMovement	2	1	عكس سند مخزن MV-2026-0002 (Out)	2026-08-07 19:55:20.413855
111	MovementReversed	StockMovement	4	1	عكس سند مخزن MV-2026-0004 (Transfer)	2026-08-07 19:55:20.442918
112	ItemUpdated	InventoryItem	1	1	تعديل صنف paracetamol_ar_v2	2026-08-07 19:55:20.542826
113	CategoryUpdated	InventoryCategory	2	1	تعديل فئة analgesics_ar_v2	2026-08-07 19:55:20.567443
114	WarehouseUpdated	Warehouse	3	1	تعديل مخزن warehouse_2_ar_v2	2026-08-07 19:55:20.594396
115	WarehouseCreated	Warehouse	0	1	إنشاء مخزن warehouse_2_ar بكود WARE-02	2026-08-07 19:59:11.744
116	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف medicines_ar	2026-08-07 19:59:11.987964
117	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف analgesics_ar	2026-08-07 19:59:12.037873
118	ItemCreated	InventoryItem	0	1	إنشاء صنف paracetamol_ar بكود ITM-001	2026-08-07 19:59:12.175113
119	ItemCreated	InventoryItem	0	1	إنشاء صنف bandage_ar بكود ITM-002	2026-08-07 19:59:12.218429
120	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0001 (In)	2026-08-07 19:59:12.386389
121	MovementPosted	StockMovement	1	1	ترحيل سند مخزن MV-2026-0001 (In)	2026-08-07 19:59:12.497289
122	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0002 (Out)	2026-08-07 19:59:12.578164
123	MovementPosted	StockMovement	2	1	ترحيل سند مخزن MV-2026-0002 (Out)	2026-08-07 19:59:12.627879
124	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0003 (Out)	2026-08-07 19:59:12.673791
125	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0004 (Transfer)	2026-08-07 19:59:12.720534
126	MovementPosted	StockMovement	4	1	ترحيل سند مخزن MV-2026-0004 (Transfer)	2026-08-07 19:59:12.737259
127	MovementReversed	StockMovement	2	1	عكس سند مخزن MV-2026-0002 (Out)	2026-08-07 19:59:12.978533
128	MovementReversed	StockMovement	4	1	عكس سند مخزن MV-2026-0004 (Transfer)	2026-08-07 19:59:13.019291
129	ItemUpdated	InventoryItem	1	1	تعديل صنف paracetamol_ar_v2	2026-08-07 19:59:13.190487
130	CategoryUpdated	InventoryCategory	2	1	تعديل فئة analgesics_ar_v2	2026-08-07 19:59:13.21121
131	WarehouseUpdated	Warehouse	4	1	تعديل مخزن warehouse_2_ar_v2	2026-08-07 19:59:13.239288
132	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف medicines_ar	2026-08-07 20:00:27.103789
133	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف analgesics_ar	2026-08-07 20:00:27.122986
134	CategoryUpdated	InventoryCategory	4	1	تعديل فئة analgesics_ar_v2	2026-08-07 20:00:27.466427
135	WarehouseCreated	Warehouse	0	1	إنشاء مخزن warehouse_2_ar بكود WARE-02	2026-08-07 20:00:41.000507
136	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف medicines_ar	2026-08-07 20:00:41.065849
137	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف analgesics_ar	2026-08-07 20:00:41.075255
138	ItemCreated	InventoryItem	0	1	إنشاء صنف paracetamol_ar بكود ITM-001	2026-08-07 20:00:41.127552
139	ItemCreated	InventoryItem	0	1	إنشاء صنف bandage_ar بكود ITM-002	2026-08-07 20:00:41.149754
140	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0001 (In)	2026-08-07 20:00:41.245115
141	MovementPosted	StockMovement	1	1	ترحيل سند مخزن MV-2026-0001 (In)	2026-08-07 20:00:41.287885
142	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0002 (Out)	2026-08-07 20:00:41.325352
143	MovementPosted	StockMovement	2	1	ترحيل سند مخزن MV-2026-0002 (Out)	2026-08-07 20:00:41.345356
144	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0003 (Out)	2026-08-07 20:00:41.367276
145	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0004 (Transfer)	2026-08-07 20:00:41.393378
146	MovementPosted	StockMovement	4	1	ترحيل سند مخزن MV-2026-0004 (Transfer)	2026-08-07 20:00:41.40617
147	MovementReversed	StockMovement	2	1	عكس سند مخزن MV-2026-0002 (Out)	2026-08-07 20:00:41.476664
148	MovementReversed	StockMovement	4	1	عكس سند مخزن MV-2026-0004 (Transfer)	2026-08-07 20:00:41.500588
149	ItemUpdated	InventoryItem	1	1	تعديل صنف paracetamol_ar_v2	2026-08-07 20:00:41.554428
150	CategoryUpdated	InventoryCategory	2	1	تعديل فئة analgesics_ar_v2	2026-08-07 20:00:41.569885
151	WarehouseUpdated	Warehouse	5	1	تعديل مخزن warehouse_2_ar_v2	2026-08-07 20:00:41.580857
152	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف med_ar	2026-08-07 20:13:59.320187
153	ItemCreated	InventoryItem	0	1	إنشاء صنف p1_ar بكود ITM-001	2026-08-07 20:13:59.549806
154	ItemCreated	InventoryItem	0	1	إنشاء صنف p2_ar بكود ITM-002	2026-08-07 20:13:59.596684
155	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0001 (In)	2026-08-07 20:13:59.784781
156	MovementPosted	StockMovement	5	1	ترحيل سند مخزن MV-2026-0001 (In)	2026-08-07 20:13:59.885146
157	StockCountCreated	StockCount	0	1	إنشاء سند جرد CNT-2026-0001	2026-08-07 20:14:00.04271
160	StockCountCreated	StockCount	0	1	إنشاء سند جرد CNT-2026-0002	2026-08-07 20:14:00.812989
161	StockCountUpdated	StockCount	2	1	تعديل سند جرد CNT-2026-0002	2026-08-07 20:14:00.868207
162	StockCountUpdated	StockCount	1	1	تعديل سند جرد CNT-2026-0001	2026-08-07 20:14:00.920602
163	StockCountCreated	StockCount	0	1	إنشاء سند جرد CNT-2026-0003	2026-08-07 20:14:25.79154
165	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف med_ar	2026-08-07 20:16:08.089492
166	ItemCreated	InventoryItem	0	1	إنشاء صنف p1_ar بكود ITM-001	2026-08-07 20:16:08.31286
167	ItemCreated	InventoryItem	0	1	إنشاء صنف p2_ar بكود ITM-002	2026-08-07 20:16:08.353632
168	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0001 (In)	2026-08-07 20:16:08.542585
169	MovementPosted	StockMovement	1	1	ترحيل سند مخزن MV-2026-0001 (In)	2026-08-07 20:16:08.639086
170	StockCountCreated	StockCount	0	1	إنشاء سند جرد CNT-2026-0001	2026-08-07 20:16:08.809046
171	StockCountPosted	StockCount	1	1	ترحيل سند جرد CNT-2026-0001 مع تسوية أرصدة تلقائية	2026-08-07 20:16:09.121122
172	StockCountReversed	StockCount	1	1	عكس سند جرد CNT-2026-0001	2026-08-07 20:16:09.376586
173	StockCountCreated	StockCount	0	1	إنشاء سند جرد CNT-2026-0002	2026-08-07 20:16:09.513662
174	StockCountUpdated	StockCount	2	1	تعديل سند جرد CNT-2026-0002	2026-08-07 20:16:09.559357
175	StockCountPosted	StockCount	2	1	ترحيل سند جرد CNT-2026-0002 مع تسوية أرصدة تلقائية	2026-08-07 20:16:09.608692
176	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف med_ar	2026-08-07 20:16:33.557097
177	ItemCreated	InventoryItem	0	1	إنشاء صنف p1_ar بكود ITM-001	2026-08-07 20:16:33.576801
178	ItemCreated	InventoryItem	0	1	إنشاء صنف p2_ar بكود ITM-002	2026-08-07 20:16:33.591275
179	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0001 (In)	2026-08-07 20:16:33.663421
180	MovementPosted	StockMovement	1	1	ترحيل سند مخزن MV-2026-0001 (In)	2026-08-07 20:16:33.679876
181	StockCountCreated	StockCount	0	1	إنشاء سند جرد CNT-2026-0001	2026-08-07 20:16:33.766785
182	StockCountPosted	StockCount	1	1	ترحيل سند جرد CNT-2026-0001 مع تسوية أرصدة تلقائية	2026-08-07 20:16:33.91645
183	StockCountReversed	StockCount	1	1	عكس سند جرد CNT-2026-0001	2026-08-07 20:16:34.039059
184	StockCountCreated	StockCount	0	1	إنشاء سند جرد CNT-2026-0002	2026-08-07 20:16:34.109839
185	StockCountUpdated	StockCount	2	1	تعديل سند جرد CNT-2026-0002	2026-08-07 20:16:34.126521
186	StockCountPosted	StockCount	2	1	ترحيل سند جرد CNT-2026-0002 مع تسوية أرصدة تلقائية	2026-08-07 20:16:34.161064
225	VoucherCreated	Voucher	0	1	إنشاء سند RC-2026-0009 (Receipt) بمبلغ 250.00 د.ل	2026-08-07 20:52:44.542126
226	VoucherCreated	Voucher	0	1	إنشاء سند PY-2026-0001 (Payment) بمبلغ 80.00 د.ل	2026-08-07 20:52:44.561321
227	VoucherPosted	Voucher	14	1	ترحيل سند RC-2026-0009 بقيمة 250.00 د.ل — قيد JE-2026-0015	2026-08-07 20:52:44.57784
228	VoucherPosted	Voucher	15	1	ترحيل سند PY-2026-0001 بقيمة 80.00 د.ل — قيد JE-2026-0016	2026-08-07 20:52:44.649641
229	TreasuryCreated	Treasury	0	1	إنشاء خزينة TESTBOX بكود SMK-02	2026-08-07 20:52:44.664687
230	VoucherCreated	Voucher	0	1	إنشاء سند TR-2026-0001 (Transfer) بمبلغ 50.00 د.ل	2026-08-07 20:52:44.677183
231	VoucherPosted	Voucher	16	1	ترحيل سند TR-2026-0001 بقيمة 50.00 د.ل — قيد JE-2026-0017	2026-08-07 20:52:44.693738
232	FiscalClosureSet	SystemSetting	0	1	الإقفال المالي حتى 2000-01-31	2026-08-07 20:52:44.808511
233	VoucherCreated	Voucher	0	1	إنشاء سند RC-2026-0010 (Receipt) بمبلغ 10.00 د.ل	2026-08-07 20:52:44.819935
234	FiscalClosureOpened	SystemSetting	0	1	فتح الإقفال المالي (إلغاء القفل)	2026-08-07 20:52:44.864922
301	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف analgesics_ar	2026-08-07 20:58:19.441581
235	VoucherReversed	Voucher	14	1	عكس سند RC-2026-0009 بقيمة 250.00 د.ل — قيد JE-2026-0018	2026-08-07 20:52:44.878476
236	JournalEntryCreated	JournalEntry	0	1	إنشاء قيد JE-2026-0019 بمبلغ 100.00 د.ل — Test entry: cash deposit	2026-08-07 20:52:45.643493
237	JournalEntryPosted	JournalEntry	26	1	ترحيل قيد JE-2026-0019 بقيمة 100.00 د.ل	2026-08-07 20:52:45.717439
239	VoucherCreated	Voucher	0	1	إنشاء سند PY-2026-0002 (Payment) بمبلغ 80.00 د.ل	2026-08-07 20:52:53.572834
240	VoucherPosted	Voucher	19	1	ترحيل سند PY-2026-0002 بقيمة 80.00 د.ل — قيد JE-2026-0020	2026-08-07 20:52:53.589045
242	FiscalClosureOpened	SystemSetting	0	1	فتح الإقفال المالي (إلغاء القفل)	2026-08-07 20:52:53.712027
244	VoucherCreated	Voucher	0	1	إنشاء سند PY-2026-0003 (Payment) بمبلغ 80.00 د.ل	2026-08-07 20:53:19.213285
245	VoucherPosted	Voucher	22	1	ترحيل سند PY-2026-0003 بقيمة 80.00 د.ل — قيد JE-2026-0021	2026-08-07 20:53:19.236839
247	FiscalClosureOpened	SystemSetting	0	1	فتح الإقفال المالي (إلغاء القفل)	2026-08-07 20:53:19.359174
249	VoucherCreated	Voucher	0	1	إنشاء سند RC-2026-0001 (Receipt) بمبلغ 250.00 د.ل	2026-08-07 20:55:47.761798
250	VoucherCreated	Voucher	0	1	إنشاء سند PY-2026-0001 (Payment) بمبلغ 80.00 د.ل	2026-08-07 20:55:47.776838
251	VoucherPosted	Voucher	25	1	ترحيل سند RC-2026-0001 بقيمة 250.00 د.ل — قيد JE-2026-0001	2026-08-07 20:55:47.790191
252	VoucherPosted	Voucher	26	1	ترحيل سند PY-2026-0001 بقيمة 80.00 د.ل — قيد JE-2026-0002	2026-08-07 20:55:47.855186
253	TreasuryCreated	Treasury	0	1	إنشاء خزينة TESTBOX بكود SMK-02	2026-08-07 20:55:47.863834
254	VoucherCreated	Voucher	0	1	إنشاء سند TR-2026-0001 (Transfer) بمبلغ 50.00 د.ل	2026-08-07 20:55:47.873219
255	VoucherPosted	Voucher	27	1	ترحيل سند TR-2026-0001 بقيمة 50.00 د.ل — قيد JE-2026-0003	2026-08-07 20:55:47.884168
256	FiscalClosureSet	SystemSetting	0	1	الإقفال المالي حتى 2000-01-31	2026-08-07 20:55:47.920551
257	VoucherCreated	Voucher	0	1	إنشاء سند RC-2026-0002 (Receipt) بمبلغ 10.00 د.ل	2026-08-07 20:55:47.928283
258	FiscalClosureOpened	SystemSetting	0	1	فتح الإقفال المالي (إلغاء القفل)	2026-08-07 20:55:47.970245
259	VoucherReversed	Voucher	25	1	عكس سند RC-2026-0001 بقيمة 250.00 د.ل — قيد JE-2026-0004	2026-08-07 20:55:47.979953
261	VoucherCreated	Voucher	0	1	إنشاء سند PY-2026-0002 (Payment) بمبلغ 80.00 د.ل	2026-08-07 20:55:55.630037
262	VoucherPosted	Voucher	30	1	ترحيل سند PY-2026-0002 بقيمة 80.00 د.ل — قيد JE-2026-0005	2026-08-07 20:55:55.648289
264	FiscalClosureOpened	SystemSetting	0	1	فتح الإقفال المالي (إلغاء القفل)	2026-08-07 20:55:55.795462
266	VoucherCreated	Voucher	0	1	إنشاء سند RC-2026-0001 (Receipt) بمبلغ 250.00 د.ل	2026-08-07 20:57:02.076972
267	VoucherCreated	Voucher	0	1	إنشاء سند PY-2026-0001 (Payment) بمبلغ 80.00 د.ل	2026-08-07 20:57:02.090936
268	VoucherPosted	Voucher	33	1	ترحيل سند RC-2026-0001 بقيمة 250.00 د.ل — قيد JE-2026-0001	2026-08-07 20:57:02.101611
269	VoucherPosted	Voucher	34	1	ترحيل سند PY-2026-0001 بقيمة 80.00 د.ل — قيد JE-2026-0002	2026-08-07 20:57:02.159073
270	TreasuryCreated	Treasury	0	1	إنشاء خزينة TESTBOX بكود SMK-02	2026-08-07 20:57:02.166357
271	VoucherCreated	Voucher	0	1	إنشاء سند TR-2026-0001 (Transfer) بمبلغ 50.00 د.ل	2026-08-07 20:57:02.176699
272	VoucherPosted	Voucher	35	1	ترحيل سند TR-2026-0001 بقيمة 50.00 د.ل — قيد JE-2026-0003	2026-08-07 20:57:02.190584
273	FiscalClosureSet	SystemSetting	0	1	الإقفال المالي حتى 2000-01-31	2026-08-07 20:57:02.224629
274	VoucherCreated	Voucher	0	1	إنشاء سند RC-2026-0002 (Receipt) بمبلغ 10.00 د.ل	2026-08-07 20:57:02.231681
275	FiscalClosureOpened	SystemSetting	0	1	فتح الإقفال المالي (إلغاء القفل)	2026-08-07 20:57:02.278918
276	VoucherReversed	Voucher	33	1	عكس سند RC-2026-0001 بقيمة 250.00 د.ل — قيد JE-2026-0004	2026-08-07 20:57:02.29078
277	JournalEntryCreated	JournalEntry	0	1	إنشاء قيد JE-2026-0005 بمبلغ 100.00 د.ل — Test entry: cash deposit	2026-08-07 20:57:16.802659
278	JournalEntryPosted	JournalEntry	38	1	ترحيل قيد JE-2026-0005 بقيمة 100.00 د.ل	2026-08-07 20:57:16.827205
279	WarehouseCreated	Warehouse	0	1	إنشاء مخزن warehouse_2_ar بكود WARE-02	2026-08-07 20:58:02.707594
280	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف medicines_ar	2026-08-07 20:58:02.797044
281	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف analgesics_ar	2026-08-07 20:58:02.829167
282	ItemCreated	InventoryItem	0	1	إنشاء صنف paracetamol_ar بكود ITM-001	2026-08-07 20:58:02.92676
283	ItemCreated	InventoryItem	0	1	إنشاء صنف bandage_ar بكود ITM-002	2026-08-07 20:58:02.953168
284	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0001 (In)	2026-08-07 20:58:03.097481
285	MovementPosted	StockMovement	5	1	ترحيل سند مخزن MV-2026-0001 (In)	2026-08-07 20:58:03.194599
286	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0002 (Out)	2026-08-07 20:58:03.232678
287	MovementPosted	StockMovement	6	1	ترحيل سند مخزن MV-2026-0002 (Out)	2026-08-07 20:58:03.254696
288	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0003 (Out)	2026-08-07 20:58:03.274796
289	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0004 (Out)	2026-08-07 20:58:03.28493
290	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0005 (Transfer)	2026-08-07 20:58:03.310553
291	MovementPosted	StockMovement	9	1	ترحيل سند مخزن MV-2026-0005 (Transfer)	2026-08-07 20:58:03.321677
292	MovementReversed	StockMovement	9	1	عكس سند مخزن MV-2026-0005 (Transfer)	2026-08-07 20:58:03.49716
293	ItemUpdated	InventoryItem	3	1	تعديل صنف paracetamol_ar_v2	2026-08-07 20:58:03.564836
294	CategoryUpdated	InventoryCategory	3	1	تعديل فئة analgesics_ar_v2	2026-08-07 20:58:03.580797
295	WarehouseUpdated	Warehouse	6	1	تعديل مخزن warehouse_2_ar_v2	2026-08-07 20:58:03.602431
296	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف medicines_ar	2026-08-07 20:58:05.794748
297	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف analgesics_ar	2026-08-07 20:58:05.806614
298	CategoryUpdated	InventoryCategory	5	1	تعديل فئة analgesics_ar_v2	2026-08-07 20:58:06.018599
299	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف med_ar	2026-08-07 20:58:08.045625
300	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف medicines_ar	2026-08-07 20:58:19.43127
302	CategoryUpdated	InventoryCategory	8	1	تعديل فئة analgesics_ar_v2	2026-08-07 20:58:19.693212
303	WarehouseCreated	Warehouse	0	1	إنشاء مخزن warehouse_2_ar بكود WARE-02	2026-08-07 20:59:23.133786
304	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف medicines_ar	2026-08-07 20:59:23.189473
305	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف analgesics_ar	2026-08-07 20:59:23.19746
306	ItemCreated	InventoryItem	0	1	إنشاء صنف paracetamol_ar بكود ITM-001	2026-08-07 20:59:23.220488
307	ItemCreated	InventoryItem	0	1	إنشاء صنف bandage_ar بكود ITM-002	2026-08-07 20:59:23.237589
308	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0001 (In)	2026-08-07 20:59:23.301549
309	MovementPosted	StockMovement	10	1	ترحيل سند مخزن MV-2026-0001 (In)	2026-08-07 20:59:23.323604
310	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0002 (Out)	2026-08-07 20:59:23.35947
311	MovementPosted	StockMovement	11	1	ترحيل سند مخزن MV-2026-0002 (Out)	2026-08-07 20:59:23.372428
312	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0003 (Out)	2026-08-07 20:59:23.391251
313	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0004 (Out)	2026-08-07 20:59:23.402082
314	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0005 (Transfer)	2026-08-07 20:59:23.421401
315	MovementPosted	StockMovement	14	1	ترحيل سند مخزن MV-2026-0005 (Transfer)	2026-08-07 20:59:23.43187
316	MovementReversed	StockMovement	14	1	عكس سند مخزن MV-2026-0005 (Transfer)	2026-08-07 20:59:23.510819
317	ItemUpdated	InventoryItem	5	1	تعديل صنف paracetamol_ar_v2	2026-08-07 20:59:23.566925
318	CategoryUpdated	InventoryCategory	10	1	تعديل فئة analgesics_ar_v2	2026-08-07 20:59:23.577878
319	WarehouseUpdated	Warehouse	7	1	تعديل مخزن warehouse_2_ar_v2	2026-08-07 20:59:23.58759
320	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف medicines_ar	2026-08-07 20:59:25.728665
321	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف analgesics_ar	2026-08-07 20:59:25.738961
322	CategoryUpdated	InventoryCategory	12	1	تعديل فئة analgesics_ar_v2	2026-08-07 20:59:25.960513
323	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف med_ar	2026-08-07 20:59:27.943028
324	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف medicines_ar	2026-08-07 20:59:37.840322
325	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف analgesics_ar	2026-08-07 20:59:37.850548
326	CategoryUpdated	InventoryCategory	15	1	تعديل فئة analgesics_ar_v2	2026-08-07 20:59:38.132815
327	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف medicines_ar	2026-08-07 20:59:40.193786
328	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف analgesics_ar	2026-08-07 20:59:40.204866
329	WarehouseCreated	Warehouse	0	1	إنشاء مخزن warehouse_2_ar بكود WARE-02	2026-08-07 20:59:52.709117
330	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف medicines_ar	2026-08-07 20:59:52.762963
331	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف analgesics_ar	2026-08-07 20:59:52.773113
332	ItemCreated	InventoryItem	0	1	إنشاء صنف paracetamol_ar بكود ITM-001	2026-08-07 20:59:52.793618
333	ItemCreated	InventoryItem	0	1	إنشاء صنف bandage_ar بكود ITM-002	2026-08-07 20:59:52.804038
334	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0001 (In)	2026-08-07 20:59:52.856735
335	MovementPosted	StockMovement	15	1	ترحيل سند مخزن MV-2026-0001 (In)	2026-08-07 20:59:52.871924
336	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0002 (Out)	2026-08-07 20:59:52.90303
337	MovementPosted	StockMovement	16	1	ترحيل سند مخزن MV-2026-0002 (Out)	2026-08-07 20:59:52.910692
338	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0003 (Out)	2026-08-07 20:59:52.929344
339	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0004 (Out)	2026-08-07 20:59:52.939565
340	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0005 (Transfer)	2026-08-07 20:59:52.959402
341	MovementPosted	StockMovement	19	1	ترحيل سند مخزن MV-2026-0005 (Transfer)	2026-08-07 20:59:52.968904
342	MovementReversed	StockMovement	19	1	عكس سند مخزن MV-2026-0005 (Transfer)	2026-08-07 20:59:53.038699
343	ItemUpdated	InventoryItem	7	1	تعديل صنف paracetamol_ar_v2	2026-08-07 20:59:53.083657
344	CategoryUpdated	InventoryCategory	19	1	تعديل فئة analgesics_ar_v2	2026-08-07 20:59:53.090825
345	WarehouseUpdated	Warehouse	8	1	تعديل مخزن warehouse_2_ar_v2	2026-08-07 20:59:53.096656
346	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف medicines_ar	2026-08-07 21:00:00.97299
347	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف analgesics_ar	2026-08-07 21:00:00.98244
348	CategoryUpdated	InventoryCategory	21	1	تعديل فئة analgesics_ar_v2	2026-08-07 21:00:01.237974
349	WarehouseCreated	Warehouse	0	1	إنشاء مخزن warehouse_2_ar بكود WARE-02	2026-08-07 21:00:28.455333
350	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف medicines_ar	2026-08-07 21:00:28.502857
351	CategoryCreated	InventoryCategory	0	1	إنشاء فئة أصناف analgesics_ar	2026-08-07 21:00:28.511275
352	ItemCreated	InventoryItem	0	1	إنشاء صنف paracetamol_ar بكود ITM-001	2026-08-07 21:00:28.531244
353	ItemCreated	InventoryItem	0	1	إنشاء صنف bandage_ar بكود ITM-002	2026-08-07 21:00:28.540139
354	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0001 (In)	2026-08-07 21:00:28.592899
355	MovementPosted	StockMovement	20	1	ترحيل سند مخزن MV-2026-0001 (In)	2026-08-07 21:00:28.608468
356	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0002 (Out)	2026-08-07 21:00:28.634391
357	MovementPosted	StockMovement	21	1	ترحيل سند مخزن MV-2026-0002 (Out)	2026-08-07 21:00:28.642612
358	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0003 (Out)	2026-08-07 21:00:28.661291
359	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0004 (Out)	2026-08-07 21:00:28.672671
360	MovementCreated	StockMovement	0	1	إنشاء سند مخزن MV-2026-0005 (Transfer)	2026-08-07 21:00:28.690955
361	MovementPosted	StockMovement	24	1	ترحيل سند مخزن MV-2026-0005 (Transfer)	2026-08-07 21:00:28.697918
362	MovementReversed	StockMovement	24	1	عكس سند مخزن MV-2026-0005 (Transfer)	2026-08-07 21:00:28.762327
363	ItemUpdated	InventoryItem	9	1	تعديل صنف paracetamol_ar_v2	2026-08-07 21:00:28.805089
364	CategoryUpdated	InventoryCategory	23	1	تعديل فئة analgesics_ar_v2	2026-08-07 21:00:28.810478
365	WarehouseUpdated	Warehouse	9	1	تعديل مخزن warehouse_2_ar_v2	2026-08-07 21:00:28.817065
366	PatientFilesMerged	PatientProfile	16	1	دمج ملف مريض Mohamed Khalid Fathi (رقم الملف PT-2026-0016) إلى ملف Ali Hassan Salem Mansour (رقم الملف PT-2026-0015)	2026-08-07 22:01:31.831045
367	PatientFilesMerged	PatientProfile	21	1	دمج ملف مريض Mohamed Khalid Fathi (رقم الملف PT-2026-0021) إلى ملف Ali Hassan Salem Mansour (رقم الملف PT-2026-0020)	2026-08-07 22:04:13.48147
368	EmployeeCreated	Employee	0	1	إنشاء بطاقة موظف Smoke Emp Fixed_1786204940 برقم EMP-2026-0001	2026-08-08 18:02:21.049109
369	EmployeeCreated	Employee	0	1	إنشاء بطاقة موظف Smoke Emp Doctor_1786204940 برقم EMP-2026-0002	2026-08-08 18:02:21.80164
370	EmployeeUpdated	Employee	1	1	تعديل بطاقة موظف Smoke Emp Fixed Updated_1786204940	2026-08-08 18:02:21.982384
371	EmployeeDeactivated	Employee	1	1	تعطيل موظف Smoke Emp Fixed Updated_1786204940	2026-08-08 18:02:22.171862
372	EmployeeActivated	Employee	1	1	تفعيل موظف Smoke Emp Fixed Updated_1786204940	2026-08-08 18:02:22.217317
373	EmployeeCourseAdded	EmployeeCourse	0	1	إضافة دورة «First Aid Smoke_1786204940» لموظف Smoke Emp Fixed Updated_1786204940	2026-08-08 18:02:22.485369
374	LeaveRequested	EmployeeLeave	0	1	طلب إجازة (Annual) لموظف Smoke Emp Fixed Updated_1786204940 لمدة 5 يوم	2026-08-08 18:02:22.681575
375	LeaveApproved	EmployeeLeave	1	1	اعتماد إجازة موظف Smoke Emp Fixed Updated_1786204940	2026-08-08 18:02:22.868886
376	LeaveRequested	EmployeeLeave	0	1	طلب إجازة (Sick) لموظف Smoke Emp Fixed Updated_1786204940 لمدة 3 يوم	2026-08-08 18:02:22.910223
377	LeaveRejected	EmployeeLeave	2	1	رفض إجازة موظف Smoke Emp Fixed Updated_1786204940	2026-08-08 18:02:22.943172
378	PayrollRun	SalaryRecord	0	1	توليد مسودة رواتب شهر 8/2026 — 2 موظف	2026-08-08 18:02:23.092562
379	PayrollRun	SalaryRecord	0	1	توليد مسودة رواتب شهر 8/2026 — 0 موظف	2026-08-08 18:02:23.187086
380	PayrollPosted	SalaryRecord	1	1	ترحيل راتب Smoke Emp Fixed Updated_1786204940 (1,550.00 د.ل) — شهر 8/2026	2026-08-08 18:02:23.69835
381	PayrollReversed	SalaryRecord	1	1	عكس راتب Smoke Emp Fixed Updated_1786204940 — شهر 8/2026	2026-08-08 18:02:23.855602
382	InvoicePaidCash	Invoice	20	1	تم تحصيل الفاتورة رقم #20 نقداً بقيمة 250.00 دينار ليبي في الاستقبال.	2026-08-08 18:02:24.772929
383	InvoiceJournalAuto	JournalEntry	0	1	قيد تلقائي لتحصيل فاتورة #20 بقيمة 250.00 د.ل	2026-08-08 18:02:24.892691
384	FiscalClosureSet	SystemSetting	0	1	الإقفال المالي حتى 2026-08-08	2026-08-08 18:02:25.083168
385	FiscalClosureOpened	SystemSetting	0	1	فتح الإقفال المالي (إلغاء القفل)	2026-08-08 18:02:25.142873
386	InvoicePaidCash	Invoice	19	1	تم تحصيل الفاتورة رقم #19 نقداً بقيمة 0.00 دينار ليبي في الاستقبال.	2026-08-08 18:02:25.183414
387	EmployeeCreated	Employee	0	1	إنشاء بطاقة موظف Test Doctor برقم EMP-2026-0003	2026-08-09 19:17:42.767289
388	EmployeeCreated	Employee	0	1	إنشاء بطاقة موظف حمزه حمزه برقم EMP-2026-0003	2026-08-09 19:27:12.944963
389	LeaveApproved	EmployeeLeave	4	1	اعتماد إجازة موظف حمزه حمزه	2026-08-09 19:32:12.952309
390	PayrollRun	SalaryRecord	0	1	توليد مسودة رواتب شهر 8/2026 — 1 موظف	2026-08-09 19:32:53.663402
391	AppointmentCreated	Appointment	21	9	تم حجز موعد جديد مع الطبيب طه احمد (درجة الفرز: 45)	2026-08-27 18:12:35.889185
\.


--
-- Data for Name: Beds; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."Beds" ("BedID", "RoomID", "BedNumber", "Status", "Notes") FROM stdin;
1	1	B101-1	Occupied	سرير عناية فاخر
2	2	B102-1	Occupied	سرير عادي جانبي
3	2	B102-2	Vacant	سرير عادي نافذة
4	3	B201-1	Vacant	سرير خاص مفرد
5	4	BICU-1	Vacant	سرير عناية مركزة مجهز بمراقبة حيوية
\.


--
-- Data for Name: ChartAccounts; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."ChartAccounts" ("AccountID", "AccountCode", "AccountName", "AccountNameAr", "AccountType", "ParentAccountID", "OpeningBalance", "IsActive", "CreatedAt") FROM stdin;
1	1000	Assets	الأصول	Asset	\N	0.00	t	2026-08-21 14:25:45.483743
2	1010	Cash on Hand	الصندوق (النقدية)	Asset	1	0.00	t	2026-08-21 14:25:45.483744
3	1020	Bank Accounts	البنوك	Asset	1	0.00	t	2026-08-21 14:25:45.483744
4	1030	Accounts Receivable (Patients)	حسابات قبض (مرضى)	Asset	1	0.00	t	2026-08-21 14:25:45.483744
5	1100	Inventory	المخزون (أدوية ومواد)	Asset	1	0.00	t	2026-08-21 14:25:45.483745
6	2000	Liabilities	الخصوم	Liability	\N	0.00	t	2026-08-21 14:25:45.483745
7	2010	Accounts Payable (Suppliers)	حسابات دائنة (موردون)	Liability	6	0.00	t	2026-08-21 14:25:45.483745
8	2020	Accrued Salaries	رواتب ومستحقات مستحقة	Liability	6	0.00	t	2026-08-21 14:25:45.483745
9	2030	Accrued Doctor Commissions	عمولات أطباء مستحقة	Liability	6	0.00	t	2026-08-21 14:25:45.483745
10	3000	Equity	حقوق الملكية	Equity	\N	0.00	t	2026-08-21 14:25:45.483746
11	3010	Owner's Capital	رأس المال	Equity	10	0.00	t	2026-08-21 14:25:45.483746
12	3020	Retained Earnings	أرباح أو خسائر مرحّلة	Equity	10	0.00	t	2026-08-21 14:25:45.483747
13	4000	Revenues	الإيرادات	Revenue	\N	0.00	t	2026-08-21 14:25:45.483747
14	4010	Consultation Revenue	إيرادات الكشوفات والعيادة	Revenue	13	0.00	t	2026-08-21 14:25:45.483747
15	4020	Pharmacy Revenue	إيرادات الصيدلية	Revenue	13	0.00	t	2026-08-21 14:25:45.483747
16	4030	Laboratory Revenue	إيرادات المختبر	Revenue	13	0.00	t	2026-08-21 14:25:45.483747
17	4040	Radiology Revenue	إيرادات الأشعة	Revenue	13	0.00	t	2026-08-21 14:25:45.483748
18	4050	Inpatient Revenue	إيرادات الإيواء والتنويم	Revenue	13	0.00	t	2026-08-21 14:25:45.483748
19	5000	Expenses	المصروفات	Expense	\N	0.00	t	2026-08-21 14:25:45.483757
20	5010	Salaries Expense	مصروف رواتب الموظفين	Expense	19	0.00	t	2026-08-21 14:25:45.483757
21	5020	Doctor Commissions Expense	مصروف عمولات الأطباء	Expense	19	0.00	t	2026-08-21 14:25:45.483757
22	5030	Rent Expense	مصروف الإيجار	Expense	19	0.00	t	2026-08-21 14:25:45.483758
23	5040	Utilities Expense	مصروف الكهرباء والماء	Expense	19	0.00	t	2026-08-21 14:25:45.483758
24	5050	Maintenance Expense	مصروف الصيانة والتجهيزات	Expense	19	0.00	t	2026-08-21 14:25:45.483758
25	5060	General Expense	مصروفات عامة ومتنوعة	Expense	19	0.00	t	2026-08-21 14:25:45.48376
\.


--
-- Data for Name: CultureSensitivities; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."CultureSensitivities" ("CultureSensitivityID", "LabOrderItemID", "Organism", "GramStain", "CultureStatus", "QuantitativeResult", "CreatedAt") FROM stdin;
\.


--
-- Data for Name: CustomAssessmentTemplates; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."CustomAssessmentTemplates" ("TemplateID", "DoctorID", "Title", "Description", "SchemaJson", "TemplateType", "IsStandard", "MaxScore", "IsActive", "CreatedAt") FROM stdin;
1	\N	مقياس الصحة العامة للاكتئاب (PHQ-9)	استبيان عالمي معياري لقياس شدة أعراض الاكتئاب خلال آخر أسبوعين. يتألف من 9 أسئلة ويستغرق 3 دقائق.	{\n  "questions": [\n    { "id": 1, "text": "نشاط أو اهتمام أقل بالأشياء عادةً ما تستمتع بها", "type": "scoring", "options": ["لا إطلاقاً = 0", "عدة أيام = 1", "أكثر من نصف الأيام = 2", "تقريباً كل يوم = 3"], "weights": [0, 1, 2, 3] },\n    { "id": 2, "text": "شعور بالاكتئاب أو اليأس أو قلة الأمل", "type": "scoring", "options": ["لا إطلاقاً = 0", "عدة أيام = 1", "أكثر من نصف الأيام = 2", "تقريباً كل يوم = 3"], "weights": [0, 1, 2, 3] },\n    { "id": 3, "text": "صعوبة في النوم أو البقاء نائماً أو النوم المفرط", "type": "scoring", "options": ["لا إطلاقاً = 0", "عدة أيام = 1", "أكثر من نصف الأيام = 2", "تقريباً كل يوم = 3"], "weights": [0, 1, 2, 3] },\n    { "id": 4, "text": "الشعور بالإرهايد أو ضعف الطاقة", "type": "scoring", "options": ["لا إطلاقاً = 0", "عدة أيام = 1", "أكثر من نصف الأيام = 2", "تقريباً كل يوم = 3"], "weights": [0, 1, 2, 3] },\n    { "id": 5, "text": "قلة الشهية أو الإفراط في الأكل", "type": "scoring", "options": ["لا إطلاقاً = 0", "عدة أيام = 1", "أكثر من نصف الأيام = 2", "تقريباً كل يوم = 3"], "weights": [0, 1, 2, 3] },\n    { "id": 6, "text": "تقدير سلبى لذاتك (أشعر أنني فاشل أو لقد خيّبت ظروف عائلتي)", "type": "scoring", "options": ["لا إطلاقاً = 0", "عدة أيام = 1", "أكثر من نصف الأيام = 2", "تقريباً كل يوم = 3"], "weights": [0, 1, 2, 3] },\n    { "id": 7, "text": "صعوبة في التركيز على الأنشطة مثل القراءة أو التلفاز", "type": "scoring", "options": ["لا إطلاقاً = 0", "عدة أيام = 1", "أكثر من نصف الأيام = 2", "تقريباً كل يوم = 3"], "weights": [0, 1, 2, 3] },\n    { "id": 8, "text": "تتحرك أو تتحدث ببطء لدرجة ملاحظة الآخرين، أو العكس، تتحرك بضجر أكثر من المعتاد", "type": "scoring", "options": ["لا إطلاقاً = 0", "عدة أيام = 1", "أكثر من نصف الأيام = 2", "تقريباً كل يوم = 3"], "weights": [0, 1, 2, 3] },\n    { "id": 9, "text": "أفكار بأنك قد تتأذى أو أنك قد تؤذى نفسك بطريقة ما", "type": "scoring", "options": ["لا إطلاقاً = 0", "عدة أيام = 1", "أكثر من نصف الأيام = 2", "تقريباً كل يوم = 3"], "weights": [0, 1, 2, 3] }\n  ],\n  "scoring": {\n    "min": 0,\n    "max": 27,\n    "ranges": [\n      { "min": 0,  "max": 4,  "label": "الحد الأدنى من أعراض الاكتئاب",        "color": "#2DC653", "recommendation": "لا يتطلب تدخلاً علاجياً، مراقبة دورية." },\n      { "min": 5,  "max": 9,  "label": "أعراض اكتئاب خفيفة",                 "color": "#FF9F1C", "recommendation": "يُوصى بالمتابعة مع طبيب مختص للدعم النفسي." },\n      { "min": 10, "max": 14, "label": "أعراض اكتئاب متوسطة",                "color": "#FF6B35", "recommendation": "توصية بتقييم سريري وعلاج دوائي محتمل." },\n      { "min": 15, "max": 19, "label": "أعراض اكتئاب متوسطة الشدة",          "color": "#E63946", "recommendation": "توصية بعلاج دوائي فوري + علاج سلوكي معرفي." },\n      { "min": 20, "max": 27, "label": "أعراض اكتئاب شديدة",                 "color": "#9B2D30", "recommendation": "توصية عاجلة بتدخل طبي نفسي مكثف وتقييم خطر السلوك الانتحاري." }\n    ]\n  }\n}	PHQ9	t	27	t	2026-01-01 00:00:00
2	\N	مقياس القلق المعمم (GAD-7)	استبيان عالمي معياري لقياس شدة القلق والتوتر خلال آخر أسبوعين. يتألف من 7 أسئلة ويستغرق دقيقتين.	{\n  "questions": [\n    { "id": 1, "text": "الشعور بالتوتر أو القلق أو العصبية", "type": "scoring", "options": ["لا إطلاقاً = 0", "عدة أيام = 1", "أكثر من نصف الأيام = 2", "تقريباً كل يوم = 3"], "weights": [0, 1, 2, 3] },\n    { "id": 2, "text": "عدم القدرة على إيقاف القلق أو التحكم به", "type": "scoring", "options": ["لا إطلاقاً = 0", "عدة أيام = 1", "أكثر من نصف الأيام = 2", "تقريباً كل يوم = 3"], "weights": [0, 1, 2, 3] },\n    { "id": 3, "text": "القلق المفرط على أشياء مختلفة", "type": "scoring", "options": ["لا إطلاقاً = 0", "عدة أيام = 1", "أكثر من نصف الأيام = 2", "تقريباً كل يوم = 3"], "weights": [0, 1, 2, 3] },\n    { "id": 4, "text": "صعوبة في الاسترخاء", "type": "scoring", "options": ["لا إطلاقاً = 0", "عدة أيام = 1", "أكثر من نصف الأيام = 2", "تقريباً كل يوم = 3"], "weights": [0, 1, 2, 3] },\n    { "id": 5, "text": "الشعور بالضجر لدرجة يصعب الجلوس في مكان", "type": "scoring", "options": ["لا إطلاقاً = 0", "عدة أيام = 1", "أكثر من نصف الأيام = 2", "تقريباً كل يوم = 3"], "weights": [0, 1, 2, 3] },\n    { "id": 6, "text": "الشعور بالانزعاج أو توقع حدوث شيء سيء بسهولة", "type": "scoring", "options": ["لا إطلاقاً = 0", "عدة أيام = 1", "أكثر من نصف الأيام = 2", "تقريباً كل يوم = 3"], "weights": [0, 1, 2, 3] },\n    { "id": 7, "text": "الشعور بالخوف أو الرعب بدون سبب واضح", "type": "scoring", "options": ["لا إطلاقاً = 0", "عدة أيام = 1", "أكثر من نصف الأيام = 2", "تقريباً كل يوم = 3"], "weights": [0, 1, 2, 3] }\n  ],\n  "scoring": {\n    "min": 0,\n    "max": 21,\n    "ranges": [\n      { "min": 0,  "max": 4,  "label": "الحد الأدنى من أعراض القلق",        "color": "#2DC653", "recommendation": "لا يتطلب تدخلاً علاجياً، مراقبة دورية." },\n      { "min": 5,  "max": 9,  "label": "أعراض قلق خفيفة",                 "color": "#FF9F1C", "recommendation": "يُوصى بالمتابعة مع طبيب مختص للدعم النفسي." },\n      { "min": 10, "max": 14, "label": "أعراض قلق متوسطة",                "color": "#FF6B35", "recommendation": "توصية بتقييم سريري وعلاج دوائي محتمل." },\n      { "min": 15, "max": 21, "label": "أعراض قلق شديدة",                 "color": "#E63946", "recommendation": "توصية عاجلة بتدخل طبي نفسي مكثف وتقييم خطر الحالة." }\n    ]\n  }\n}	GAD7	t	21	t	2026-01-01 00:00:00
3	3	الحالة الصحية	اختبار حالة	[{"text":"هل لديك اعراض جديدة","type":"text"}]	Custom	f	\N	t	2026-07-11 03:39:36.64343
4	\N	استبيان الصحة العام	تقييم عام	{"questions":[{"id":1,"text":"كيف تشعر؟"}]}	Custom	f	\N	t	2026-08-06 20:37:02.736381
\.


--
-- Data for Name: DispenseRecords; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."DispenseRecords" ("DispenseID", "PrescriptionID", "MedicationID", "QuantityDispensed", "TotalPrice", "DispensedByUserID", "Status", "Notes", "DispensedAt") FROM stdin;
1	3	2	1	15.00	5	Dispensed	اخذ الدواء بانتظام	2026-07-09 18:53:05.169999
2	4	1	1	20.00	5	Dispensed		2026-07-11 03:32:25.517944
3	6	\N	1	0.00	26	Dispensed	\N	2026-08-06 19:49:32.440505
\.


--
-- Data for Name: DoctorCommissions; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."DoctorCommissions" ("CommissionID", "DoctorID", "Specialty", "ServiceID", "CommissionType", "Value", "CreatedAt") FROM stdin;
1	8		\N	Percentage	50.00	2026-07-31 12:36:43.813229
2	11		\N	Percentage	50.00	2026-08-03 17:24:42.723781
3	23	\N	\N	Percentage	30.00	2026-08-06 20:44:52.961058
\.


--
-- Data for Name: DoctorProfiles; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."DoctorProfiles" ("DoctorID", "UserID", "Specialty", "LicenseNumber", "EmergencyReady", "Bio", "ImageUrl", "AvailableDays", "WorkStartTime", "WorkEndTime", "ConsultationDurationMinutes", "ConsultationFee") FROM stdin;
1	3	طب عام	12879658	f	\N	\N	\N	\N	\N	30	50.00
2	7	انف و حنجرة	878412161	f	\N	\N	\N	\N	\N	30	100.00
3	8	الطب النفسي	58946136	f	\N	\N	\N	\N	\N	30	100.00
4	11	الطب النفسي	6431316	f	\N	\N	\N	\N	\N	30	100.00
5	13	طب انف و حنجرة	68786435	f	\N	\N	Sun,Tue,Thu	09:00:00	17:00:00	30	100.00
6	18	الطب النفسي	87946133	f	\N	\N	\N	\N	\N	30	100.00
7	23	اختبار	TEST001	f	\N	\N	\N	\N	\N	30	100.00
8	45	Clinic	\N	f	\N	\N	\N	\N	\N	30	100.00
\.


--
-- Data for Name: EmployeeCourses; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."EmployeeCourses" ("CourseID", "EmployeeID", "CourseName", "Provider", "CourseDate", "CertificateNumber", "ExpiryDate", "Notes", "CreatedAt") FROM stdin;
\.


--
-- Data for Name: EmployeeLeaves; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."EmployeeLeaves" ("LeaveID", "EmployeeID", "LeaveType", "StartDate", "EndDate", "Days", "Reason", "Status", "ApprovedByUserID", "ApprovedAt", "CreatedAt") FROM stdin;
1	1	Annual	2026-07-01 00:00:00	2026-07-05 00:00:00	5	smoke test	Approved	1	2026-08-08 18:02:22.868823	2026-08-08 18:02:22.653889
2	1	Sick	2026-08-01 00:00:00	2026-08-03 00:00:00	3	smoke reject	Rejected	1	2026-08-08 18:02:22.943162	2026-08-08 18:02:22.910015
3	2	Annual	2026-09-01 00:00:00	2026-09-02 00:00:00	2	self request	Pending	\N	\N	2026-08-08 18:02:24.341102
4	4	Unpaid	2026-08-09 00:00:00	2026-08-19 00:00:00	11	ظروف عائلية	Approved	1	2026-08-09 19:32:12.952211	2026-08-09 19:30:48.006359
\.


--
-- Data for Name: EmployeeProfiles; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."EmployeeProfiles" ("EmployeeID", "UserID", "EmployeeNumber", "FullName", "Department", "Position", "HireDate", "Gender", "NationalID", "CompensationModel", "BaseSalary", "BankAccount", "IsActive", "Notes", "CreatedAt") FROM stdin;
1	\N	EMP-2026-0001	Smoke Emp Fixed Updated_1786204940	Front Desk	Senior Receptionist	2026-01-15 00:00:00	Female	NID-FIXED-1786204940	FixedSalary	1500.00	BANK-1786204940	t	\N	2026-08-08 18:02:20.99318
2	45	EMP-2026-0002	Smoke Emp Doctor_1786204940	Clinic	Doctor	2026-08-08 00:00:00	Male	\N	Commission	0.00	\N	t	\N	2026-08-08 18:02:21.800064
4	13	EMP-2026-0003	حمزه حمزه	طب انف و حنجرة	طبيب	2026-08-01 00:00:00	Male	11999012547	FixedSalary	1000.00	0100547892655	t		2026-08-09 19:27:12.871967
\.


--
-- Data for Name: HealthServices; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."HealthServices" ("ServiceID", "ServiceName", "ServiceNameAr", "Category", "Description", "Price", "Unit", "IsActive", "CreatedAt") FROM stdin;
1	Nursing Care	????? ????	Nursing	???? ????? ????? ??????	50.00	???	t	2026-08-21 12:30:03.71803
2	nursing care	تمريض	Nursing	ممرض يقدم للطبيب اهتمام دوري	15.00	مرتين في اليوم	t	2026-08-27 14:42:42.641609
\.


--
-- Data for Name: InpatientCareExecutions; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."InpatientCareExecutions" ("ExecutionID", "OrderID", "ExecutedByUserID", "ExecutedAt", "Status", "Notes", "VitalTemperature", "VitalBloodPressure", "VitalPulse", "VitalOxygen") FROM stdin;
1	3	1	2026-08-06 20:32:47.761535	Executed	تم	36.6			
2	2	13	2026-08-13 01:34:41.397611	Executed	تم اعطاء المريض الجرعة	34	33	32	03%
\.


--
-- Data for Name: InpatientCareOrders; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."InpatientCareOrders" ("OrderID", "AdmissionID", "HealthServiceID", "OrderType", "OrderDescription", "Frequency", "ScheduledTime", "UnitPrice", "Status", "CreatedAt", "CreatedByUserID") FROM stdin;
1	2	\N	IVFluid	تغذية	Every4Hours	2026-07-26 11:30:00	15.00	Pending	2026-07-25 19:39:37.883693	1
2	1	\N	Medication	مضاد في الوريد	Every4Hours	2026-07-27 15:15:00	20.00	Executed	2026-07-25 19:41:47.236057	1
3	3	\N	Medication	جرعة اختبار	Once	2026-08-06 18:32:47.621	50.00	Executed	2026-08-06 20:32:47.66628	23
\.


--
-- Data for Name: InpatientDailyLogs; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."InpatientDailyLogs" ("LogID", "AdmissionID", "LoggedByUserID", "LogDate", "Temperature", "BloodPressure", "PulseRate", "OxygenLevel", "DoctorNotes", "NursingNotes") FROM stdin;
1	1	13	2026-07-24 13:20:07.045322	37.0	120/80	75	98%	يجب ابقاءه تحت الملاحظته ليوم اخر	في تحسن
2	3	23	2026-08-06 20:32:47.541446	36.6	120/80	72	98	حالة مستقرة	
3	3	1	2026-08-06 20:32:47.761674	36.6				\N	تنفيذ أمر رعاية (جرعة اختبار): تم
4	1	13	2026-08-13 01:34:41.398781	34	33	32	03%	\N	تنفيذ أمر رعاية (مضاد في الوريد): تم اعطاء المريض الجرعة
\.


--
-- Data for Name: InventoryCategories; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."InventoryCategories" ("CategoryID", "CategoryName", "CategoryNameAr", "ParentCategoryID", "IsActive", "CreatedAt") FROM stdin;
\.


--
-- Data for Name: InventoryItems; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."InventoryItems" ("ItemID", "ItemCode", "ItemName", "ItemNameAr", "CategoryID", "MedicationID", "Unit", "PurchasePrice", "SellingPrice", "ReorderLevel", "Manufacturer", "ExpiryDate", "IsActive", "CreatedAt") FROM stdin;
\.


--
-- Data for Name: Invoices; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."Invoices" ("InvoiceID", "PatientUserID", "AppointmentID", "DispenseRecordID", "LabOrderID", "RadiologyOrderID", "InvoiceType", "Amount", "Tax", "Discount", "TotalAmount", "Status", "PaymentMethod", "TransactionReference", "CreatedAt", "PaidAt", "DoctorShare", "ClinicShare", "DoctorID", "DoctorCommissionID") FROM stdin;
1	2	4	\N	\N	\N	Consultation	0.00	0.00	0.00	0.00	Paid	Cash	CASH-F5616044	2026-05-22 14:13:01.320279	2026-05-31 18:43:11.034078	0.00	0.00	\N	\N
2	6	5	\N	\N	\N	Consultation	100.00	15.00	0.00	115.00	Paid	Card	TXN-B02856C3	2026-05-22 15:12:17.871711	2026-05-29 14:24:01.132389	0.00	0.00	\N	\N
3	6	6	\N	\N	\N	Consultation	0.00	0.00	0.00	0.00	Unpaid	\N	\N	2026-05-23 15:32:53.896146	\N	0.00	0.00	\N	\N
4	2	7	\N	\N	\N	Consultation	50.00	7.50	0.00	57.50	Unpaid	\N	\N	2026-05-24 19:11:42.722224	\N	0.00	0.00	\N	\N
5	2	\N	1	\N	\N	Pharmacy	15.00	2.25	0.00	17.25	Paid	Cash	CASH-EE0118DA	2026-07-09 18:53:05.350216	2026-07-09 18:54:57.989162	0.00	0.00	\N	\N
6	4	8	\N	\N	\N	Consultation	100.00	15.00	0.00	115.00	Unpaid	\N	\N	2026-07-11 02:05:52.750516	\N	0.00	0.00	\N	\N
7	4	\N	2	\N	\N	Pharmacy	20.00	0.00	0.00	20.00	Unpaid	\N	\N	2026-07-11 03:32:26.215332	\N	0.00	0.00	\N	\N
8	9	9	\N	\N	\N	Consultation	100.00	15.00	0.00	115.00	Unpaid	\N	\N	2026-07-11 03:59:15.198937	\N	0.00	0.00	\N	\N
9	10	10	\N	\N	\N	Consultation	100.00	15.00	0.00	115.00	Unpaid	\N	\N	2026-07-14 11:29:31.883913	\N	0.00	0.00	\N	\N
10	10	11	\N	\N	\N	Consultation	100.00	15.00	0.00	115.00	Unpaid	\N	\N	2026-07-14 11:41:53.469968	\N	0.00	0.00	\N	\N
11	12	12	\N	\N	\N	Consultation	100.00	0.00	0.00	100.00	Unpaid	\N	\N	2026-07-15 14:42:13.066882	\N	0.00	0.00	\N	\N
12	14	13	\N	\N	\N	Consultation	100.00	0.00	0.00	100.00	Paid	Cash	\N	2026-07-31 14:38:12.712097	2026-07-31 14:38:12.711886	50.00	50.00	8	1
13	7	14	\N	\N	\N	Consultation	100.00	0.00	0.00	100.00	Unpaid	Cash	\N	2026-08-04 20:54:31.642253	\N	50.00	50.00	13	\N
14	9	15	\N	\N	\N	Consultation	100.00	0.00	0.00	100.00	Paid	Cash	CASH-5D9EB9E0	2026-08-04 21:47:36.20327	2026-08-04 21:52:20.113824	50.00	50.00	13	\N
15	21	16	\N	\N	\N	Consultation	100.00	0.00	0.00	100.00	Unpaid	Cash	\N	2026-08-04 22:55:28.084052	\N	50.00	50.00	13	\N
16	21	17	\N	\N	\N	Consultation	50.00	0.00	0.00	50.00	Unpaid	Cash	\N	2026-08-04 23:18:56.920988	\N	25.00	25.00	3	\N
17	10	18	\N	\N	\N	Consultation	50.00	0.00	0.00	50.00	Unpaid	\N	\N	2026-08-06 17:56:50.697446	\N	25.00	25.00	3	\N
18	22	19	\N	\N	\N	Consultation	100.00	0.00	0.00	100.00	Paid	Card	TXN-A15F879D	2026-08-06 19:38:19.62964	2026-08-06 19:39:54.877633	50.00	50.00	23	\N
19	10	\N	3	\N	\N	Pharmacy	0.00	0.00	0.00	0.00	Paid	Cash	CASH-7D1C59D2	2026-08-06 19:49:32.555663	2026-08-08 18:02:25.183385	0.00	0.00	\N	\N
20	22	\N	\N	\N	\N	Inpatient	250.00	0.00	0.00	250.00	Paid	Cash	CASH-3DCAB6F6	2026-08-06 20:32:48.012797	2026-08-08 18:02:24.772365	0.00	0.00	\N	\N
21	28	20	\N	\N	\N	Consultation	60.00	0.00	0.00	60.00	Paid	Cash	\N	2026-08-06 20:45:05.194928	2026-08-06 20:45:05.194779	18.00	42.00	23	3
9002	9	21	\N	\N	\N	Consultation	50.00	0.00	0.00	50.00	Unpaid	\N	\N	2026-08-27 18:12:35.943748	\N	25.00	25.00	3	\N
\.


--
-- Data for Name: JournalEntries; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."JournalEntries" ("JournalEntryID", "EntryNumber", "EntryDate", "Description", "SourceModule", "SourceReferenceID", "Status", "CreatedByUserID", "CreatedAt", "PostedAt", "PostedByUserID") FROM stdin;
39	JE-2026-0001	2026-08-31 00:00:00	استحقاق راتب Smoke Emp Fixed Updated_1786204940 — شهر 8/2026	Salary	1	Posted	1	2026-08-08 18:02:23.58914	2026-08-08 18:02:23.589159	1
40	JE-2026-0002	2026-08-08 18:02:23.846139	عكس استحقاق راتب Smoke Emp Fixed Updated_1786204940 — شهر 8/2026	Salary	1	Posted	1	2026-08-08 18:02:23.848556	2026-08-08 18:02:23.848556	1
41	JE-2026-0003	2026-08-08 18:02:24.772365	تحصيل فاتورة #20 (Inpatient)	Invoice	20	Posted	1	2026-08-08 18:02:24.892409	2026-08-08 18:02:24.892409	1
\.


--
-- Data for Name: JournalEntryLines; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."JournalEntryLines" ("JournalEntryLineID", "JournalEntryID", "AccountID", "Debit", "Credit", "Notes") FROM stdin;
77	39	20	1550.00	0.00	Smoke Emp Fixed Updated_1786204940
78	39	8	0.00	1550.00	Smoke Emp Fixed Updated_1786204940
79	40	8	1550.00	0.00	Smoke Emp Fixed Updated_1786204940
80	40	20	0.00	1550.00	Smoke Emp Fixed Updated_1786204940
81	41	2	250.00	0.00	\N
82	41	18	0.00	250.00	\N
\.


--
-- Data for Name: LabDevices; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."LabDevices" ("LabDeviceID", "DeviceName", "DeviceCode", "DeviceModel", "ConnectionType", "IsActive", "CreatedAt") FROM stdin;
\.


--
-- Data for Name: LabOrderItems; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."LabOrderItems" ("LabOrderItemID", "LabOrderID", "LabTestID", "ResultValue", "ResultStatus", "TechnicianNotes", "CompletedAt") FROM stdin;
20	17	1	positive	High	حالته جيدة	2026-08-13 01:45:00.424199
\.


--
-- Data for Name: LabOrders; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."LabOrders" ("LabOrderID", "PatientUserID", "DoctorID", "LabTestID", "ResultValue", "ResultStatus", "Status", "ResultNotes", "TechnicianNotes", "RequestedAt", "CompletedAt", "VerificationQRCode") FROM stdin;
1	14	1	3	12.5	High	Completed	لا يوجد	جيد	2026-07-31 13:15:05.101238	2026-08-02 00:24:53.222973	CLINICPRO-LAB-1-14-639212270932313760
2	10	3	2	8.5	Low	Completed	يلزم تحليل دقيق	جيد	2026-08-02 21:03:38.302343	2026-08-02 21:10:06.042891	CLINICPRO-LAB-2-10-639213018060494920
3	22	1	1	5.5	Low	Completed	تست	تم	2026-08-06 19:50:44.453887	2026-08-06 19:50:44.676336	CLINICPRO-LAB-3-22-639216354446800728
17	9	13	1	\N	Pending	Completed	لا يوجد	\N	2026-08-13 01:30:33.487151	2026-08-13 01:45:00.424232	CLINICPRO-LAB-17-9-639221751004242389
\.


--
-- Data for Name: LabReferenceRanges; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."LabReferenceRanges" ("RangeID", "LabTestID", "Gender", "MinAge", "MaxAge", "NormalMin", "NormalMax", "RangeNotes") FROM stdin;
1	1	All	0	120	11.50	16.50	\N
2	2	All	0	120	70.00	99.00	\N
3	3	All	0	120	4.00	5.60	\N
\.


--
-- Data for Name: LabTests; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."LabTests" ("LabTestID", "TestName", "Code", "Category", "Price", "Unit", "IsPanel", "PanelID", "DeviceID", "CreatedAt") FROM stdin;
1	صورة دم كاملة (CBC)	CBC	Hematology	30.00	g/dL	f	\N	\N	2026-07-31 13:14:09.657189
2	السكر الصائم (FBS)	FBS	Biochemistry	20.00	mg/dL	f	\N	\N	2026-07-31 13:14:09.658845
3	السكر التراكمي (HbA1c)	HBA1C	Biochemistry	45.00	%	f	\N	\N	2026-07-31 13:14:09.658851
20	صورة دم كاملة	FBP	Biochemistry	50.00	mg/dl	f	\N	\N	2026-08-21 15:00:01.695482
\.


--
-- Data for Name: MedicalRecords; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."MedicalRecords" ("RecordID", "AppID", "Diagnosis", "DiagnosisAr", "TreatmentPlan", "DoctorNotes", "Symptoms", "Recommendations", "RequiresFollowUp", "FollowUpDate", "FollowUpNotes", "CreatedAt") FROM stdin;
2	1	برده	برده	الالتزام بالادوية	اكثر من السوائل	\N	\N	f	2026-05-24 22:00:00	متابعة بشكل دقيق	2026-05-20 16:02:21.133301
3	2	نزله معوية	نزله معوية	الالتزام باخذ الدواء	اكثار من السوائل	\N	\N	f	2026-05-24 22:00:00	متابعة بشكل دقيق	2026-05-21 16:41:07.30655
4	4	بردة	بردة	استراحة بالبيت	اكثر من السوائل	\N	\N	f	2026-07-14 22:00:00	اكثار السوائل	2026-07-09 18:50:01.334829
5	8	صدمة نفسية	صدمة نفسية	اخد الدواء بالنتظام	الاتزام بالدواء	\N	\N	f	2026-07-19 22:00:00	الراحة	2026-07-11 02:08:32.532665
6	9	حالة اكتئاب	حالة اكتئاب	اختلاط بناس ايجابية	التفكير بايجابية	\N	\N	f	2026-07-19 22:00:00	الراحة	2026-07-11 04:02:01.926225
7	10	بردة	بردة	اخد الدواء	الانتظام على الدواء	\N	\N	f	2026-07-24 22:00:00	الانتظام	2026-07-14 11:35:40.980813
8	19	Influenza	إنفلونزا	راحة وسوائل	متابعة	\N	\N	f	\N	\N	2026-08-06 20:35:12.729265
\.


--
-- Data for Name: MedicationRequests; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."MedicationRequests" ("RequestID", "MedicationName", "DoctorUserID", "DoctorName", "Notes", "IsResolved", "CreatedAt") FROM stdin;
1	فوار	3	طه احمد	دواء مطلوب يجب توفيره	t	2026-05-22 14:28:01.37786
2	امبرازول	3	طه احمد	الدواء مطلوب يجب توفيره	t	2026-05-22 14:47:07.073603
3	فولتارين	8	محمد مراد	الدواء مطلوب يجب توفيره	f	2026-07-14 11:35:15.723461
\.


--
-- Data for Name: Medications; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."Medications" ("MedicationID", "Name", "NameAr", "Category", "DosageForm", "Unit", "QuantityInStock", "MinStockLevel", "PurchasePrice", "SellingPrice", "Manufacturer", "ExpiryDate", "IsActive", "CreatedAt") FROM stdin;
1	panadol	بنادول	مسكن	أقراص	قرص	9	0	15.00	20.00	بنادول	2026-08-19 22:00:00	t	2026-05-22 14:24:15.883776
2	paracetamol	باراسيتامول	مسكن	كبسولات	كبسوله	11	3	10.00	15.00	باراسيتامول	2026-07-19 22:00:00	t	2026-05-22 14:26:31.885284
3	Test Med	دواء اختبار	\N	\N	\N	10	5	5.00	12.00	\N	\N	t	2026-08-06 19:49:19.754738
\.


--
-- Data for Name: PatientAssessments; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."PatientAssessments" ("AssessmentID", "PatientUserID", "TemplateID", "AnswersJson", "CreatedAt", "CompletedAt", "Status") FROM stdin;
1	4	3	{}	2026-07-11 03:39:52.816625	\N	Pending
2	9	3	{"هل لديك اعراض جديدة":"قلق متزايد و توتر مستمر"}	2026-07-11 04:05:24.594754	2026-07-11 04:06:55.489531	Completed
3	22	4	{"1":"ممتاز"}	2026-08-06 20:37:03.043611	2026-08-06 20:37:03.168696	Completed
\.


--
-- Data for Name: PatientProfiles; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."PatientProfiles" ("PatientID", "UserID", "FirstName", "FatherName", "GrandfatherName", "FamilyName", "FileNumber", "MergedIntoPatientID", "MergedAt", "BloodType", "ChronicDiseases", "Allergies", "GeneralNotes", "DateOfBirth", "Gender", "Address", "EmergencyContact", "EmergencyPhone", "RiskLevel", "RiskLevelUpdatedAt", "RiskLevelUpdatedByUserID", "RiskLevelNotes") FROM stdin;
1	2	\N	\N	\N	\N	PT-2026-0001	\N	\N	A-	\N	\N	\N	\N	ذكر	\N	\N	\N	\N	\N	\N	\N
2	4	\N	\N	\N	\N	PT-2026-0002	\N	\N	B-	\N	\N	\N	\N	ذكر	\N	\N	\N	\N	\N	\N	\N
3	6	\N	\N	\N	\N	PT-2026-0003	\N	\N	B-	\N	\N	\N	\N	ذكر	\N	\N	\N	\N	\N	\N	\N
4	9	\N	\N	\N	\N	PT-2026-0004	\N	\N	B-	\N	\N	\N	\N	ذكر	\N	\N	\N	Monitoring	2026-07-11 04:04:52.185245	8	وضعه متدهور
5	10	\N	\N	\N	\N	PT-2026-0005	\N	\N	A-	\N	\N	\N	\N	أنثى	\N	\N	\N	Stable	\N	\N	\N
6	12	\N	\N	\N	\N	PT-2026-0006	\N	\N	B+	\N	\N	\N	\N	ذكر	\N	\N	\N	Stable	\N	\N	\N
7	14	\N	\N	\N	\N	PT-2026-0007	\N	\N	\N	\N	\N	\N	\N	ذكر	\N	\N	\N	Stable	\N	\N	\N
8	17	\N	\N	\N	\N	PT-2026-0008	\N	\N	B-	\N	\N	\N	\N	ذكر	\N	\N	\N	Stable	\N	\N	\N
9	21	\N	\N	\N	\N	PT-2026-0009	\N	\N	B+	\N	\N	\N	\N	ذكر	\N	\N	\N	Stable	\N	\N	\N
10	22	\N	\N	\N	\N	PT-2026-0010	\N	\N	\N	\N	\N	\N	\N	\N	\N	\N	\N	Stable	\N	\N	\N
11	24	\N	\N	\N	\N	PT-2026-0011	\N	\N	O+	\N	\N	\N	1990-01-01 00:00:00	M	\N	\N	\N	Stable	\N	\N	\N
12	28	\N	\N	\N	\N	PT-2026-0012	\N	\N	\N	\N	\N	\N	\N	ذكر	\N	\N	\N	Stable	\N	\N	\N
\.


--
-- Data for Name: Prescriptions; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."Prescriptions" ("PrescriptionID", "RecordID", "MedicationID", "MedicationName", "Dosage", "Duration", "Instructions", "Frequency", "Quantity", "DispenseStatus", "CreatedAt") FROM stdin;
1	2	\N	بنادول	الاولى	\N	\N	3	0		2026-05-20 16:02:21.893407
2	3	\N	مضاد حيوي	الاولى	\N	\N	2	0		2026-05-21 16:41:07.913329
3	4	2	باراسيتامول (paracetamol)	500ج	\N	\N	ثلاثة مرات	1	Dispensed	2026-07-09 18:50:01.503678
4	5	1	بنادول (panadol)	10	\N	\N	2	1	Dispensed	2026-07-11 02:08:32.604719
5	6	\N	بنادول (panadol)	10	\N	\N	2	1	Pending	2026-07-11 04:02:02.060841
6	7	\N	بنادول (panadol)	50	\N	\N	2	1	Dispensed	2026-07-14 11:35:41.098376
7	8	\N	بنادول	500mg	5 أيام	بعد الأكل	3/day	1	Pending	2026-08-06 20:35:12.923848
8	8	\N	فيتامين C	1x	30 يوم		1/day	1	Pending	2026-08-06 20:35:13.099584
\.


--
-- Data for Name: Priorities; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."Priorities" ("PriorityID", "LevelName", "LevelNameAr", "Weight", "ColorCode", "Icon") FROM stdin;
1	Normal	عادي	1	#2DC653	fa-check-circle
2	Urgent	عاجل	2	#FF9F1C	fa-exclamation-triangle
3	Emergency	طوارئ	3	#E63946	fa-ambulance
\.


--
-- Data for Name: PsychiatricRecords; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."PsychiatricRecords" ("RecordID", "Appearance", "Behavior", "Speech", "MoodAndAffect", "ThoughtProcess", "ThoughtContent", "Perception", "Cognition", "InsightAndJudgment", "IsSpeechToTextUsed", "CreatedAt") FROM stdin;
5	يبدو مرتباً ونظيفاً، مهتم بمظهره	هادئ ومتعاون خلال الجلسة	كلام طبيعي في السرعة والحجم والنبرة	مزاج مبتهج أو مرح بشكل غير مناسب	مجرى تفكير منطقي ومنظم	لا توجد أوهام أو أفكار غير عادية	لا توجد هلاوس حسية	متوجه نحو الزمان والمكان والشخص	بصيرة جزئية — يقر ببعض الأعراض لكن لا يدرك خطورتها	f	2026-07-11 03:35:21.689476
6	مظهر مهمل، غير مهتم بنظافته الشخصية	متوتر وقلق، صعوبة في الجلوس	كلام بطيء ومتردد	مزاج مكتئب، حزين، باكٍ	توقف الفكر (Thought Blocking)	أوهام اضطهادية (Paranoid Delusions)	تبدد الواقع أو تبدد الشخصية	ضعف في التركيز والانتباه	حكم ضعيف على المخاطر والقرارات	f	2026-07-11 04:03:12.95395
\.


--
-- Data for Name: RadiologyOrders; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."RadiologyOrders" ("RadiologyOrderID", "PatientUserID", "DoctorID", "Modality", "BodyPart", "Status", "ReportText", "ImagePath", "RequestedAt", "CompletedAt", "RadiologistID") FROM stdin;
1	10	3	X-Ray	stomach	Completed	حالتها جيدة	\N	2026-08-02 21:04:46.263442	2026-08-02 21:06:14.10719	16
2	22	1	X-Ray	Chest	Completed	تقرير: سليمة تماماً	\N	2026-08-06 19:51:20.13184	2026-08-06 19:51:56.354	27
3	9	13	X-Ray	الرقبة	Completed	جيد	"C:\\Users\\HP\\Pictures\\efcore.jpg"	2026-08-13 01:31:47.000518	2026-08-13 01:49:27.380117	13
\.


--
-- Data for Name: RadiologyTemplates; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."RadiologyTemplates" ("TemplateID", "TemplateName", "Modality", "BodyPart", "DefaultReportText", "Price", "CreatedAt") FROM stdin;
1	أشعة سينية للصدر طبيعية (Chest X-Ray Normal)	X-Ray	Chest	PA & Lateral view of chest shows normal lung fields. Heart size is normal. Both costophrenic angles are clear. No evidence of active lung lesions.	0.00	2026-07-31 13:14:15.109673
2	موجات فوق صوتية للبطن طبيعية (Abdomen Ultrasound Normal)	Ultrasound	Abdomen	Ultrasonic examination of abdomen shows normal size, shape, and echogenicity of liver, gallbladder, spleen, pancreas, and both kidneys. No focal lesions or ascites.	0.00	2026-07-31 13:14:15.109968
\.


--
-- Data for Name: Rooms; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."Rooms" ("RoomID", "WardID", "RoomNumber", "RoomType", "DailyRate", "MaxBeds", "IsActive") FROM stdin;
1	1	101-VIP	VIP	500.00	1	t
2	1	102-A	General	200.00	2	t
3	2	201-A	Private	350.00	1	t
4	3	ICU-01	ICU	1000.00	1	t
\.


--
-- Data for Name: SalaryRecords; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."SalaryRecords" ("SalaryRecordID", "EmployeeID", "PeriodYear", "PeriodMonth", "BaseSalary", "CommissionAmount", "Bonus", "Deduction", "GrossSalary", "NetSalary", "Status", "JournalEntryID", "CreatedByUserID", "CreatedAt", "PostedAt") FROM stdin;
1	1	2026	8	1500.00	0.00	100.00	50.00	1600.00	1550.00	Reversed	39	1	2026-08-08 18:02:23.066536	2026-08-08 18:02:23.855595
2	2	2026	8	0.00	0.00	0.00	0.00	0.00	0.00	Draft	\N	1	2026-08-08 18:02:23.092425	\N
3	4	2026	8	1000.00	0.00	0.00	0.00	1000.00	1000.00	Draft	\N	1	2026-08-09 19:32:53.652032	\N
\.


--
-- Data for Name: SensitivityResults; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."SensitivityResults" ("SensitivityResultID", "CultureSensitivityID", "AntibioticName", "Interpretation", "ZoneDiameter") FROM stdin;
\.


--
-- Data for Name: SoapNotes; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."SoapNotes" ("SoapNoteID", "RecordID", "Subjective", "Objective", "Assessment", "Plan", "CreatedAt", "UpdatedAt") FROM stdin;
1	6	قلق مستمر	قلق	قلق	اخذ الدواء و مخالطة الايجابيين	2026-07-11 04:04:18.966397	2026-07-11 04:04:18.967913
\.


--
-- Data for Name: StockCountItems; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."StockCountItems" ("StockCountItemID", "StockCountID", "ItemID", "SystemQuantity", "CountedQuantity", "UnitPrice", "Notes") FROM stdin;
\.


--
-- Data for Name: StockCounts; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."StockCounts" ("StockCountID", "StockCountNumber", "CountDate", "WarehouseID", "Status", "Notes", "CreatedByUserID", "CreatedAt", "PostedByUserID", "PostedAt", "ReversedByUserID", "ReversedAt") FROM stdin;
\.


--
-- Data for Name: StockMovementItems; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."StockMovementItems" ("StockMovementItemID", "MovementID", "ItemID", "Quantity", "UnitPrice", "Notes") FROM stdin;
\.


--
-- Data for Name: StockMovements; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."StockMovements" ("MovementID", "MovementNumber", "MovementType", "MovementDate", "WarehouseID", "ToWarehouseID", "ReferenceType", "ReferenceID", "Notes", "Status", "CreatedByUserID", "CreatedAt", "PostedByUserID", "PostedAt") FROM stdin;
\.


--
-- Data for Name: SystemSettings; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."SystemSettings" ("SettingKey", "SettingValue", "UpdatedAt") FROM stdin;
CancelWindowHours	6	2026-08-21 12:25:45.468541
DefaultCommissionRatio	50	2026-08-21 12:25:45.468541
EnableMobilePWA	true	2026-08-21 12:25:45.468541
FacilityMode	General	2026-08-06 20:44:53.793734
MaxBookingDaysAhead	30	2026-08-21 12:25:45.468541
MaxFutureAppointmentsPerPatient	5	2026-08-21 12:25:45.468541
SlotBufferMinutes	5	2026-08-21 12:25:45.468542
\.


--
-- Data for Name: TelemedicineSessions; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."TelemedicineSessions" ("SessionID", "AppointmentID", "RoomCode", "Status", "CreatedByUserID", "CreatedAt", "StartedAt", "EndedAt", "SessionNotes") FROM stdin;
1	18	99E1BE64F5D6	Ended	3	2026-08-06 18:02:49.658204	2026-08-06 18:03:21.536556	2026-08-06 18:03:49.022539	\N
2	18	0F87D50FDAD6	Ended	3	2026-08-06 18:04:09.452163	2026-08-06 18:04:11.211312	2026-08-06 18:04:13.637073	\N
3	19	227026BD59CB	Ended	23	2026-08-06 20:29:17.943135	2026-08-06 20:29:18.67851	2026-08-06 20:29:18.751929	جلسة جيدة
4	19	92BB38A6CFCC	Waiting	23	2026-08-06 20:29:34.880669	\N	\N	\N
\.


--
-- Data for Name: Treasuries; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."Treasuries" ("TreasuryID", "TreasuryName", "TreasuryNameAr", "TreasuryCode", "AccountID", "IsActive", "CreatedAt") FROM stdin;
1	Main Cash	الصندوق الرئيسي	CASH-01	2	t	2026-01-01 00:00:00
2	Main Bank	الحساب البنكي الرئيسي	BANK-01	3	t	2026-01-01 00:00:00
\.


--
-- Data for Name: TriageQuestions; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."TriageQuestions" ("QuestionID", "QuestionText", "QuestionTextAr", "Weight", "Category", "IsActive", "SortOrder") FROM stdin;
1	Do you have chest pain?	هل تعاني من ألم في الصدر؟	25	Cardiac	t	1
2	Do you have difficulty breathing?	هل تعاني من صعوبة في التنفس؟	25	Respiratory	t	2
3	Do you have severe bleeding?	هل تعاني من نزيف حاد؟	20	General	t	3
4	Do you have a high fever (above 39°C)?	هل لديك حرارة مرتفعة (فوق 39 درجة)؟	15	General	t	4
5	Do you feel dizziness or loss of consciousness?	هل تشعر بدوخة أو فقدان للوعي؟	20	Neurological	t	5
6	Do you have severe abdominal pain?	هل تعاني من ألم شديد في البطن؟	15	General	t	6
7	Do you have a persistent headache?	هل تعاني من صداع مستمر؟	10	Neurological	t	7
8	Have you had a recent injury or accident?	هل تعرضت لإصابة أو حادث مؤخراً؟	15	General	t	8
9	Do you have nausea or vomiting?	هل تعاني من غثيان أو قيء؟	8	General	t	9
10	Do you have any chronic diseases?	هل لديك أمراض مزمنة؟	5	General	t	10
11	Do you feel depressed or hopeless?	هل تشعر باكتئاب أو يأس أو فقدان أمل؟	20	Psychiatric	t	11
12	Do you feel anxious or nervous most of the time?	هل تشعر بقلق أو توتر معظم الوقت؟	15	Psychiatric	t	12
13	Do you have thoughts of harming yourself or others?	هل لديك أفكار بإيذاء نفسك أو الآخرين؟	30	Psychiatric	t	13
14	Do you see or hear things that others do not?	هل ترى أو تسمع أشياء لا يراها أو يسمعها الآخرون؟	25	Psychiatric	t	14
15	Do you have trouble sleeping or changes in appetite?	هل تعاني من اضطرابات في النوم أو الشهية؟	10	Psychiatric	t	15
\.


--
-- Data for Name: UserNotifications; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."UserNotifications" ("NotificationID", "UserID", "Title", "Message", "Type", "RelatedEntityType", "RelatedEntityID", "IsRead", "CreatedAt") FROM stdin;
1	22	حان موعد جلستك ⏰	حان موعد جلستك مع د. اختبار	AppointmentTimeReached	Appointment	999	f	2026-08-06 19:22:28.53
2	22	نتيجة فحص متاحة	توجد نتيجة تحليل جديدة	LabResult	\N	\N	f	2026-08-06 19:22:28.54
3	22	مكالمة فيديو جاهزة 📹	الطبيب د. Test Doctor فتح معك جلسة فيديو — انضم الآن إلى الجلسة في انتظارك.	TelemedicineStarted	Appointment	19	f	2026-08-06 20:29:18.018634
4	22	مكالمة فيديو جاهزة 📹	الطبيب د. Test Doctor فتح معك جلسة فيديو — انضم الآن إلى الجلسة في انتظارك.	TelemedicineStarted	Appointment	19	f	2026-08-06 20:29:34.881691
5	28	حان موعد جلستك ⏰	حان موعد جلستك مع د. Test Doctor في 08:45 PM. يرجى الاستعداد، سيبدأ الطبيب مكالمة الفيديو قريباً.	AppointmentTimeReached	Appointment	20	f	2026-08-06 20:54:04.894636
\.


--
-- Data for Name: Users; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."Users" ("UserID", "FullName", "Email", "Password", "Role", "Phone", "AssignedTreasuryID", "IsActive", "CreatedAt") FROM stdin;
1	مدير النظام	admin@medical.com	$2a$11$HL3jH5enP.qhQRongvmAbO3shF9L2Hh25aK4U17IXSD/T9h3OpHMO	Admin	0500000000	\N	t	2026-01-01 00:00:00
2	احمد علي	ahmedali@gmail.com	$2a$11$.34LxTqYplwDVNg3YOd2NOrF2vXypccH9pVnv8mrj39fUyi.4eS9K	Patient	0926584781	\N	t	2026-05-04 23:56:28.46067
3	طه احمد	taha@gmail.com	$2a$11$HDAb/LusUz/BsucK4cB2O.YS3Obo4faKpJih/lREFypFj402gbmmm	Doctor	0921117878	\N	t	2026-05-04 23:58:51.639753
4	taha_aymen	tahaaymen@gmail.com	$2a$11$vlW41dj3naKtJ32WO5GKRermFfR/sWBfinVDWpuAx6VtxSlaBvO8W	Patient	0921117878	\N	t	2026-05-06 15:57:22.952432
5	ايمن محمد	aymen@gmail.com	$2a$11$QVSth7Q1aco927vNugFAv.xMLU4NBLS1M9PIwEtpyHcbkYJNE63zS	Pharmacist	0926584781	\N	t	2026-05-21 19:10:02.395677
6	ايمن محمد	aymenmohammed@gmail.com	$2a$11$rcob9g7w95BqsElnk55K9uAxRH6vAToIzH1geN5pKOFc9Z2eo2yzm	Patient	0921117878	\N	t	2026-05-21 19:30:59.734831
7	محمد علي	mohammed@gmail.com	$2a$11$lHFg3WFX8/wAplKOejrhQeqBQ.PELN2K1WxVF7v8ZWyanAHP/oBEW	Doctor	0926584788	\N	t	2026-05-22 14:21:19.273731
8	محمد مراد	mohammedmorad@gmail.com	$2a$11$sXXnrUP.ucTj3EdwjKOsMuG1yT/vwxsCd6Ay1r.yNNNk411Zt9ISu	Doctor	0921117878	\N	t	2026-07-11 01:47:42.806978
9	اسامه	osama@gmail.com	$2a$11$.cvurS8D5sJ1fnsr6HRgw.jAxt933JpwzIYg5UPwbaL7mujCp70/6	Patient	0926584788	\N	t	2026-07-11 03:57:58.065345
10	مرام	maram@gmail.com	$2a$11$UCgPkYVCxjNJW.wKKHxbq.yZDXmXqHBtgpZgxXwFQuzq1ycAd.3SS	Patient	0926584781	\N	t	2026-07-14 11:28:57.738769
11	عبد الله	abdullah@gmail.com	$2a$11$IvmPT99l9IymfPdsJ7aDH.VpE/IyHiTsw.zP/9cKqOiKlETncvC5e	Doctor	0921117878	\N	t	2026-07-14 11:40:55.435662
12	ziad	ziad@gmail.com	$2a$11$4etCz6sw4noS0Jkp.sUbj.0ven3fXFyUU.cOJVrHyuT7EW6VcNx6.	Patient	87687979	\N	t	2026-07-15 14:40:02.798467
13	hamza	hamza@gmail.com	$2a$11$STB4mGahQPpQiC05K5P3EuhZi4hXtRt.df86idYKIfITqSqyz8kaO	Doctor	092878787	\N	t	2026-07-15 14:41:24.997579
14	خالد محمد علي	khaled@gmail.com	$2a$11$eDnttqwXV6ETy33AV9E/ReWUIAyOGqq9diSICWi7oLJce2G5NVI2y	Patient	0928784986	\N	t	2026-07-31 12:38:12.491279
15	عادل	adel@gmail.com	$2a$11$7CXgMQkKoxcTQKvXikaFHOC/RFUqh0ouFpgbHhJuwqDwpMyrAUn3u	LabTechnician	0921212302	\N	t	2026-08-02 01:50:25.772179
16	كريم	karem@gmail.com	$2a$11$6Sqpbm8FZKIQ9x2Gc4UPz.jde4neWoreJIDSzZj.7lzCZlD6LSE9a	Radiologist	0927845987	\N	t	2026-08-02 01:51:07.779457
17	مالك	malek@gmail.com	$2a$11$MSyakmwboQEFurLjo2ZQueChnsMrgFdfw7FnV9ExSqeX1YCrBx1w.	Patient	09287964619	\N	t	2026-08-03 17:04:22.513907
18	انور	anwar@gmail.com	$2a$11$ehnUH2d/zauFr4BoGtx6L.vV6A.qKjWrtMr2XPOJ02toDANvEUS2m	Doctor	0928956487	\N	t	2026-08-03 17:06:00.670734
19	محمود	mahmoud@gmail.com	$2a$11$jINnkj4BLfA2AysovzbVGOYaEh5c.EtGW5Bnpt2GlkW8hY/N3VNha	Receptionist	0927848798	\N	t	2026-08-04 04:19:34.688217
20	حاتم	hatem@gmail.com	$2a$11$CaZc.240OmH9duN/ebfQDe9FsmuEON0MHXD8yfDtGTN1e5UImc5pO	Cashier	0928568497	\N	t	2026-08-04 20:45:12.481038
21	زكريا	zakaria@gmail.com	$2a$11$74/NfhLpfQmnpsbZsS.TX.C3z4w9AOs02macqG1s0IH7ih.6qmBZ2	Patient	0925487258	\N	t	2026-08-04 22:07:53.219071
22	Test Patient	testpatient@example.com	$2a$11$KAo6FKcBzcJxC0dv30LmxePvBwEwGFS73KNsqLEp1Ry.ZFM8xP2yi	Patient	\N	\N	t	2026-08-06 19:07:25.592148
23	Test Doctor	testdoctor@example.com	$2a$11$KAo6FKcBzcJxC0dv30LmxePvBwEwGFS73KNsqLEp1Ry.ZFM8xP2yi	Doctor	0511111111	\N	t	2026-08-06 19:25:26.24
24	Test Patient 2	testpatient2@example.com	$2a$11$KAo6FKcBzcJxC0dv30LmxePvBwEwGFS73KNsqLEp1Ry.ZFM8xP2yi	Patient	0522222222	\N	t	2026-08-06 19:39:02.926666
25	Test Lab	testlab@example.com	$2a$11$KAo6FKcBzcJxC0dv30LmxePvBwEwGFS73KNsqLEp1Ry.ZFM8xP2yi	LabTechnician	0533333333	\N	t	2026-08-06 19:39:40.216666
26	Test Pharm	testpharm@example.com	$2a$11$KAo6FKcBzcJxC0dv30LmxePvBwEwGFS73KNsqLEp1Ry.ZFM8xP2yi	Pharmacist	0544444444	\N	t	2026-08-06 19:39:53.5
27	????? ????	testradio@example.com	$2a$11$xxF8hgrQYHM9SLV/dV/oheLDwQCiMsn1Ip8X3G.gMNgv77TxiVkSa	Radiologist	0500000007	\N	t	2026-08-06 19:51:19.400601
28	Express Test	walkin_01970da6@clinic.com	$2a$11$NdylYCRVX8aULi6T5HXu9uPnbxKNMttgNHJLGIX3o4Rt9nRSXSt1C	Patient	0900000000	\N	t	2026-08-06 20:45:05.064619
35	ايوب	ayoup@gmail.com	$2a$11$mk0rEt.SBmGWaWVOCy9qE.OpZuH/KAWf8EtbPqdmt.1q.SGVa/Qmm	WarehouseKeeper	0927863451	\N	t	2026-08-07 21:33:22.588427
45	Smoke Emp Doctor_1786204940	empdoc_1786204940@clinic.com	$2a$11$Nuh.LQ3lI7NfJdP5KKfqzuWLqzYnRxv/4Lai7U6oy6bcNGuhNr5H6	Doctor	\N	\N	t	2026-08-08 18:02:21.718943
\.


--
-- Data for Name: Vouchers; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."Vouchers" ("VoucherID", "VoucherNumber", "VoucherType", "VoucherDate", "TreasuryID", "ToTreasuryID", "AccountID", "PatientUserID", "InvoiceID", "AppointmentID", "Amount", "Description", "Status", "CreatedByUserID", "CreatedAt", "PostedByUserID", "PostedAt") FROM stdin;
37	RC-2026-0001	Receipt	2026-08-08 18:02:24.772365	1	\N	18	22	20	\N	250.00	تحصيل فاتورة #20 (Inpatient)	Posted	1	2026-08-08 18:02:24.844351	1	2026-08-08 18:02:24.772365
38	RC-2026-0002	Receipt	2026-08-08 18:02:25.183385	1	\N	15	10	19	\N	0.00	تحصيل فاتورة #19 (Pharmacy)	Posted	1	2026-08-08 18:02:25.188177	1	2026-08-08 18:02:25.183385
\.


--
-- Data for Name: Wards; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."Wards" ("WardID", "WardName", "WardNameAr", "GenderType", "FloorNumber", "IsActive") FROM stdin;
1	Surgical Ward	جناح الجراحة العامة	Mixed	2	t
2	Internal Medicine Ward	جناح الباطنية والمرضى الداخليين	Mixed	2	t
3	Intensive Care Unit (ICU)	قسم العناية المركزة (ICU)	Mixed	3	t
\.


--
-- Data for Name: Warehouses; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."Warehouses" ("WarehouseID", "WarehouseName", "WarehouseNameAr", "WarehouseCode", "Location", "IsActive", "CreatedAt") FROM stdin;
1	Main Warehouse	المخزن الرئيسي	WARE-01	الطابق الأرضي	t	2026-01-01 00:00:00
\.


--
-- Data for Name: WebPushSubscriptions; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."WebPushSubscriptions" ("SubscriptionID", "UserID", "Endpoint", "P256DH", "Auth", "UserAgent", "IsActive", "CreatedAt", "LastUsedAt") FROM stdin;
1	10	https://fcm.googleapis.com/fcm/send/dg6J5tjXpbM:APA91bHXvd-ZZ0gqa9toI9MP_oCmaPGB-NrJY2_KyXvFfuqeXZFEjQARAca2_Fmwsqb-80Rje2hVcwLz4GgybuMbWVmm8ItRBGVgB3nG6bTHqWa1NTgg2ALa7XK3PZGE1E-Bh4nqDsLL	BGE0aAniWZ6d8+uEufjd1RdcKWyEaOnAI2iGxNN4hGn/HHG9xFvOQYivCuFFeg/+ZR6lqQrR1d8g3jVLcW1CSqo=	bGz404t1c8kyIExlICzcwg==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36	t	2026-08-06 19:16:03.296358	2026-08-06 19:16:03.296462
2	1	https://test.example.com/push/endpoint123	BElF4v1q2w3e4r5t6y7u8i9o0paaaBBBCCCDDDEEEFFFGGGHHHIIIJJJKKKLLLMMM	testAuthValue1234567890	review-test	f	2026-08-06 19:22:06.421077	2026-08-06 19:22:06.421099
3	13	https://fcm.googleapis.com/fcm/send/fBU-e_1NUV0:APA91bGgmuOD7zBfE3DNS4za8mywfK8AUOJCAoJ9jlMLSasasF55FTVFuAa10gsR8M-KlQxa1aFe8nPW054BV6zw0fkn8nMFh2ZZWoZ_x27P--Bxp2YAlcni7F3mNtnkytqyyiZHqr16	BC7vTZRKbh6cQr2hfy+Rdr5ic986FtwScZyNssEFneCKY+8xzeNGviARtcoR/Bd2IHvZYF+Utikf74xRq26amQo=	Etp40ASq6KsF6wLGYnRZ7g==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36	t	2026-08-06 19:29:46.541928	2026-08-06 19:29:46.542046
4	1	https://fcm.googleapis.com/fcm/send/fddgaaH-IiI:APA91bFQSFHYQNH-seGa8Bdy-xK27Puz9XapO4OFFbJf0C3U0Tz8tZe8_dV9IGggsB0Y7csKB4B-PY1gHyUEYjV44G8QfwCyFrfpNsrqY4NcpbIudh9ZZnI3ZsjmNa23yy3SFn-t_oip	BJ4E8pJzAbx+Hr8sL1xi1xE881hpc6EZHs4qNIN+JECfItyc93GdYc5dD1L75A5Wx3JGG14Zvli7IkJGU7VVpf0=	pk7e7ir4KqSABRM/9vG6jA==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36	t	2026-08-06 19:30:22.969135	2026-08-06 19:30:22.969135
5	23	https://fcm.googleapis.com/fcm/send/ev9FQUt1zBc:APA91bEyb5WoeOxPOHh_A9zP1bRfLAgPVOcu7lFvF6CwSmSnBr1PQSTq4yTh10CGTYIIOK2z06trh7TMF8niWFQgvZq31GlOILG7HN0u8qiOTBwOrsNhvRFMTZRNmsg1nb1EYOqn4l5H	BBOY/JN/iNw3TQ+ANnNSYUbEsYdrWtivdytL15HZi8lPFp6dy+pkB68M+nKTNaL9ZmU+h+6f1pLQDKO0dAqSKzc=	13ngE4psfHKvgPq0tNccnA==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36	t	2026-08-06 19:31:50.363429	2026-08-06 19:31:50.36343
6	1	https://fcm.googleapis.com/fcm/send/f4RDjl-qwuc:APA91bE86sZElRhqOXL9C1EpcSIFTOypxMSJX2upY8AF7goYrYptyW4qfWPUlgIowG6LqFWUvqDvX26DDm_ag8lhDUmNwW2SzDEXTNvX7MBLo9uqwNALIQ6tsIG96n_Vq2PcC3OCReDo	BB+h3KavbIpZZUVZHgBO5ICX/5uEOM29WQ1oWzYdzIgvCOBABg4JQkAnvjiJeFMmopP7zfseQN2gkNke2P8O5ys=	e2gWMUB1rKm9oRT8ompwbg==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36	t	2026-08-06 20:54:55.237538	2026-08-06 20:57:00.922337
7	1	https://fcm.googleapis.com/fcm/send/d4_9_vMVVVs:APA91bEv2NSKG_VQB48Yn2ZDwKBxCSBEYADZaFpgs9c0p6E-QzaAPjKwkk25xEi85HGskE4d4jQaRdBhBwxncZeTP5fkbOKarNdTOES2bNUaKRoA4QeuKvmOKRUnpkweaWGtUsIU3FOF	BAdWWca6PSuuMGLwVyNWZh+bGEIlPOoZMIrnHx0sy2pLpmTVYQyebTQXvWykKjd6kmLRUKzWja+I1Vw6h+uVEHQ=	99EAo1UH65xQnYA/7iNtfA==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36	t	2026-08-06 21:00:11.868322	2026-08-06 21:00:11.868322
8	10	https://fcm.googleapis.com/fcm/send/eLeztOwp5fI:APA91bFImhbqcCkoFo_Fh_FrvCH0aZOJT1oec5jFPtPEaYUb3R0lCoFR8tWK1CKUejBc8kLgaeJ0oz_A0OOrdLsYRdRBO4tKtXdrA_VgTvq0gKxOhZYXErlwsBv9yp8XsGHJGqKDopZX	BE4WIRtnKBV9/H3HXrvbbvlNUna3ZbAEzlSmn5ccJU0Sl3sON9k8qy6NpBFk3pZkrMwb+f9TogI+8sqcf32Ryb8=	wcVS5m6EiOLenTNDGrmf4A==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36	t	2026-08-07 16:30:16.022271	2026-08-07 16:31:05.341656
9	3	https://fcm.googleapis.com/fcm/send/dKYW68ICvB0:APA91bGPk5aO7UK9CdOoY_MIs58A3Q4MK-tUOvzh-SSeohFq4dL220Rx2Py-RcOG5Opr9PhXvTzg3zoaKfQAE48nd9u5HfXoIA8mN-Lz6U_BKyzUkf_gbZuVDSo9GcmLP_Yi9OkeSvOS	BL9gqDHRvFrAPT1Nf0QEkhBRiYOLTYaUXOwFCSn09XxhCeFqSLDLIPa7F1h+HYfLHHPP87N/pgtrCcBxbapiO78=	ppiJ3X3L2ivspUYQRqMdAw==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36	t	2026-08-07 16:51:20.533941	2026-08-07 16:51:20.534007
10	1	https://fcm.googleapis.com/fcm/send/dw4GzR-jN_I:APA91bGH8SeXDzHspj96mGjgDpD_HWeIEAAznPDiTgBNAlyf7VsSA-UyeOdfZK_YlmnM-sm15xZ5SYsSAQXnHQrXifvbqBZTLrDUgoNj8rGDTJRwgGAyvvYaCcJk7YV7fbJaQbIfnUq2	BAVTICRPB2QDU24AamoudbgvyoWwCZV6QkTpKXSx6HOtoRQQxmIkj9OdrPZfxJqwmxP01K1w891tudNnpnQnVYw=	10WSUFUs8lT9qGszquF71A==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36	t	2026-08-07 16:51:58.260376	2026-08-07 16:51:58.260376
11	10	https://fcm.googleapis.com/fcm/send/emhxi13_Tlk:APA91bEh5rPUjjYhybY4uMBxVutCPUAwvSp--qphnBTwdvt7hwf8IWGwm1Fqe-9q_LBwINf8ZfK2sKUfFcuSUuk_WmayneNbnqURuGWwX5VfGojciouSyj79SzqbwWczcApUBnuuFKxi	BMRtEcHAsryf37V55UAmfpbypL7A02AT8wsLJBvdC1IwwuVap0dIvZWEyKkVOlq9QjUd+FpjksMOX0KMux8Na48=	loiGYrQ4tbswEATYXbMVdA==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36	t	2026-08-07 16:52:35.759339	2026-08-07 16:53:27.522296
12	10	https://fcm.googleapis.com/fcm/send/fXkiccy6QrE:APA91bE_0WABYtfumJ6rS9ldJhzc-kSTvtrecBFXyYqCSnu4uWiMwPBfNCbW_Cm8SM72kpSPahlSTqSszJ1HJpDULmWasDOR47W7J3FGRCdBiKwU2AtUfHRzjDktnKfFSPNm6iwdFc7A	BEHs+6XZpOehi7ZXDolS9pijMOZmjLj8SZ9jY+UeUN28E49M82yGJ5Q6fIguYuS+PpMlpZOmFRd3WJlTVebnxH4=	USj4jcI0RTKAvVmIkMzLoA==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36	t	2026-08-07 17:06:14.839465	2026-08-07 17:06:34.586301
13	1	https://fcm.googleapis.com/fcm/send/eq0dvq0OhCE:APA91bFEJaUXJzivN7LYyK5DC6AN1l1GPmYuzK9VAbmD5jUR9dJfb7DTfizFLlmuoyaZlnKxiiXgh5PsbbMlW6lGhY7Ux7DyOn79l_qQzeKsXkOweOIXOAlESHrY7NAVHZaYyjciQTGV	BDlnS5dq6YHATb5lwr472qOIPBIJ/CnofGR85VB/32DMOPXKVjVEoRiQIzKCsWaSKE9NNSmw6cRv8wgKgn2O6eQ=	TITt/UnkyvwHBSxesq+rcw==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36	t	2026-08-07 20:02:22.118562	2026-08-07 20:02:22.118595
14	1	https://fcm.googleapis.com/fcm/send/cUiVxZdvbT4:APA91bHcA59xkALkctsx_mpBQzLWMPw260XkYfsPMQARPWfoCk2ivsNeX9XJRaCpsFYnpV4JtIdogqC2XwmvFYISsACVGqRgYDilvumUPTNgDTuA512jS56eUikE--qf3PsuhLfx33Nx	BF8cvvsVLUpgMvjy79R0aXGDcLQyMb5+X3/kxNKFhFq1aJsV+LP/1kzDnDNp9irnkzDyzRUaNziAdtTX1H6MRdE=	wv/kuHKkSx/P9RHUpo9rUw==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36	t	2026-08-07 20:20:47.54065	2026-08-07 20:20:47.540698
15	1	https://fcm.googleapis.com/fcm/send/exs36qv8w5I:APA91bHdOdHJn3r5kCW7pVH2xgGwXeisQkcbjvEJgLg1Jy5q0UgQ0Q0ElJMMDxnaIr1tqswMLamJRAQDYPDnWAb3EBpgTbkMEx0hvQsidVbtuYOqWorkeDHAybMrGqU9GYUZtgTRmJMj	BP48nqckgUlkwuyzQ/+SL+XehpAYA2lVTZoCbfky/q9qKqnluStKuvnJQTezqD08Z9P3vLyhRbUYV/QJCU+lSn4=	dzBkGhlADuY5C8To0FDp0A==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36	t	2026-08-07 20:30:23.726938	2026-08-07 20:30:23.727253
16	1	https://fcm.googleapis.com/fcm/send/cTkgMCes37Q:APA91bEfVR0a17s20wOdvVgXJkB5jYwhPKPy4YYq2fGD7f-eSbv7wdPTzDmm4hNTWpSkkC3gc21K24igQbZ6O7ZHIPrdQKYjHqI8ZUqLOA7gjHqCav04CWjuESI4w7fJL7hselz2qJcO	BDbcDQvjSDiLzgAOomBe7rd26ROgI61Ee0bFdMZ46CaoZLQYnAarEAbkYjD9RvNteLTtOIVwWzM4SwOVj0k/8W0=	/dzCUGa2/TXnDCTbl7cXIQ==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36	t	2026-08-07 21:31:28.524066	2026-08-07 21:31:28.524091
17	35	https://fcm.googleapis.com/fcm/send/f-zAr8JXaU4:APA91bGG_vYxh51aO6TnQGdeY5F4QWApI3MNOYDQ2Ez9IV1IVNLvG_t4NMVNrqFm6UkI2xTqeYlJw_3zGdhRXOvSnCCNnK1q1cJhIlosc9hzpkVX1buC8clOw7h8vxpCLjWpWsCVdLeF	BOTbZhoEI3+/ozy+9DqxzAByOxSE6oZ3t7eBopzXHX5ruY6XLidMQvirM8mT29XchYbc3iyEWPbEM/xTUG1INko=	1C0Nic1YGRNtYYGwT2JTLw==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36	t	2026-08-07 21:34:12.517498	2026-08-07 21:34:16.33778
18	1	https://fcm.googleapis.com/fcm/send/cScNnr8dbcs:APA91bF1qhzt_X0N_StmL9yisI1jHIetIKVIKRj1UZ9wK2yxhc6emnMa7jUgAFp2HM9n9SpSTtRcWBb5fginYqavVCHAH_rn71h4XXS7CboUqAWjmfSgLpI6MC3lrKGeK0WxVsvc0I6u	BJNBzsHglW8VTzgBKh88AZR3zZhWND1aUZiNh4drH6KMD30VCdUt23NZNA+pkxF0NwrTR457rHn3isUws+GrSrY=	mXBZjnRTFsDnSyNMSu0HVA==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/150.0.0.0 Safari/537.36	t	2026-08-07 21:40:26.683834	2026-08-07 21:40:26.683879
19	1	https://fcm.googleapis.com/fcm/send/eCGWljfsGAc:APA91bGpMqMMoayLV1X4_NAc97D28ftYc80QJczvAI-YlomuoVqsn-iG8XngXb2P5A56yuJbGECaCjYVPLRN6XTd1u5M5Z-VsNKj1QJszt4aEKM3XQJPoN5cWvs33nFQU8sb_brVwp2d	BLPm9lGG4IIrMajSHbjEliorexb7yXF8ow7O0BKZIXNmmhWuzEOfTrdXeKm3BqA2dsxfYIoYyzhjmeOjX3Btlek=	2YkJURNckBEWIHJshlggcQ==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-08 18:14:49.135746	2026-08-08 18:15:36.062385
20	1	https://fcm.googleapis.com/fcm/send/chm6Al_TTVY:APA91bEPdB_jRMwC9O9jTmJNRA4S50cwZdUJ6Cp45yk5BlReciwxrZkvgf7CjSsylKI7xK_Tmi73JZx5L8LZZWWLchDI45hRW_dGH8wvjFbbrUml2w0kgwp70tvjktYT2vpVrSaF5Wt8	BA5qzQjX7Cf/QrOMDJedN4LoLwDVrWmYZwHCXAnRmMW/zy2kooxiI9+JjwQzhgRTNHHBAHm+11p75ViHZw29Hko=	1DZ4szbAzewr2tdVbKvjbg==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-08 18:32:05.213069	2026-08-08 18:32:05.213165
21	1	https://fcm.googleapis.com/fcm/send/cC8JO37eTO4:APA91bHSZHxkm_cVMNa_bVulXgM8q-udJXHDd__KdA4LDTRO-cCuL41NMSfRNyswDWRqvevv3ic4dNgJfcGcTQNUNqAUlynLdMEMJdHPc-kBuk_yQNII2OdFdnQkR9DS7ZLRi_VDgosX	BAoRrzl6+JOdtCS9QXvV5rz0hxcz/74Q6ZumslerrSnQ1YWnHHc3lkUAUzR/WnGz9gNOJM+sYiBE11Wq7zHbgB4=	WvpLKh/gvVG+3XoFggImoA==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-08 18:46:37.642025	2026-08-08 18:50:14.807624
22	1	https://fcm.googleapis.com/fcm/send/ckNT7-sLeLI:APA91bF9RmVVlkY5PlSJX3UOYvVogqqvOZKRXlVEM0DLnf0szXIjNTJ0-YGaUFT4Acpy0zUkFpMySYz2oUYh51bf1Df1azYl1KhxoEHu6CwMux0XFJGsrby0fcqMA8mjlSAGfMRwcm71	BM8lq4glHr2knGoWYoazTqB/+/tU5ILwvHRAzmmYHGSEBED2JTxVGFsAfN6AiR2feaqFx/LQVty2O4w9uEaFk2U=	3UAQfWGe9H1YWD8rUQZ3ZQ==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-08 19:02:50.096749	2026-08-08 19:02:50.096831
23	1	https://fcm.googleapis.com/fcm/send/d3OR5T9pefE:APA91bEd3YirOXq53rnFgVW2bR8d47Ulr4J9ofkwRxGcrEN0JmNokU646qPQuXYxqUM7F2slufC7jNByEW5xi44NGJ7FrqiwTqtasf2RLa-pf7QyPWTFLRa01VFhKaIt9dDsmZh3n_UP	BApcdcsoohcbGQ64X0JpUcJb3lHG+5Bg4uql+LEnSg7O8eAD/UTG1D3XN4EtI0bUJrscZK5Gk6mro5saS4XF7yg=	Z2+LcEiYtr1jiyqQQcIBmQ==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-08 20:41:32.29855	2026-08-08 20:41:32.298612
24	1	https://fcm.googleapis.com/fcm/send/cdMvi_u_PPQ:APA91bHOhLcHYjbeyBsgiwl3jNl-9jk6xNut-q2asFVB81CXjsVFWYjqSmqb3JaHRc9wMSJuqYh_rm99bJ8HP8UYobHsSxGFuE4j7crCZ14cUK6P0xwhz5ZiDpj3V5UhNL7A8L6vrzRX	BGOjO8mjSHUU6a7grPlXfuetrGJzJ3d3h67+Kf8bdCN7K5LQ+jOEy9mnus1zDAgZYnHSBPaKrh2hwg1ObgqIw28=	pZBC3F01g1eF9wwhFYXrJQ==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-08 20:56:30.1833	2026-08-08 20:56:30.183353
25	1	https://fcm.googleapis.com/fcm/send/dzxGMe31vis:APA91bFLwWXA-lR_zLAG58QTHo42NERST8bZZ-GMr-elp-CvRxI6Yz4SMgfFK_a8SLjwDLAELr0w6VND_bt_ucCkc2ZxI5RBMLambom4s1btMb3CPTMpAinjPIbt484KeAr1n5Kgau_4	BFJTjdCgrbwLXTJcb7duhKP0TPNCWqOnaaPL6w0E7lBwxnEqzOp2Uwm233wp0l2p0SscWxH0UVoMeuZ/qcn+TWw=	VbEltS/l0ZbxIjKuI23b6w==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-08 21:10:51.271134	2026-08-08 21:10:59.322273
26	1	https://fcm.googleapis.com/fcm/send/cK8ITU7mZLc:APA91bFR9IITEgAdaqcN4zpfOUgUuKxtqwgDNopLsaQWB2lrGK0rIZwyeMbahUczuknO6Nf8EPIxAiAt6PQ-jDsazk041ok_S9UdzrOYyWxRVVyq2wc6N88dsT19swqreYFAyEeXedtk	BPeyfOKKBp1lWerVFQscNlwk2Qddx04rx3XPfvb7eLIRXhq2RRI/WIpadOZQBp1crvGJnXjEp7EChdN5QjUKv3U=	QDFE2KG5W9UF2mzSD4vJBg==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-08 21:23:25.617456	2026-08-08 21:23:43.715219
27	1	https://fcm.googleapis.com/fcm/send/e-NdZNybwVI:APA91bE8twjDhlQP5Op_vfmYm4flCL6q91hjld_A2deoYpikLD-G-eYpd3H1SdXMulKuPoKHTWgCLqoQwWS6QM3g2b5Hdox38SzadEZAEnOQLWx9GslodFzb1cbam5Zz7yKkXEiLx_eT	BHEjFWcOcekOFBc6DlPb1AVu0GeXBsjNJKg5OFsl9esdTCm897nvrC08F9hLImiydXt+nEhJVN24OvDyAzRxoYM=	jRRITwCLFc1XO5DY8O9Hcw==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-08 21:37:41.747565	2026-08-08 21:38:14.290987
28	1	https://fcm.googleapis.com/fcm/send/cWpTgyFqZbQ:APA91bE_vKnyttaIf3z5Gl4LAKk8n1540wIhJ-EmH-e_fXCirKizqPKGdIeK9BJg5V71NnR_YytpuY0x3Vb-yInye-vsfZ-UDNILNsPR1Tq4Nzw9VY4SKkrJZw7NzhXlQ3jSeU76RtL4	BMO8yYQNNyILDm2iAdM6+MWe2J4NkLjYygFENGeJZFOCoxlhAP87j+yoBGf11O73u+xuwZOUdLM6GxJdikgH7d8=	OvnBhMYB7T6KPuxVW+IxwQ==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-08 21:43:10.779306	2026-08-08 21:43:24.090224
29	1	https://fcm.googleapis.com/fcm/send/eHsOSoZhas8:APA91bHEIPt10fR8gOpvVZQ-J188gn_B8F7X7EhnBAlfU7p4nGYBkvyU_cV2mGIR9WrJmQGk2ceX5Y3nQeZFDWeZpeCBeS52YaN-TKMwwlZcCf34Hs99ZZvVBCOiCUtaZUepr-VBWZ5B	BIZD3WtV+9JQLATPz8s9YVpX7qYsVS1dTLZ8k6bzr2NlVMdQovb2eIl32x2P5nIvBVXl+nu5zcDnGqCIGB3cQQQ=	Gk878IgwqUTEFRO2rRrAsA==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-09 18:36:18.866214	2026-08-09 18:36:29.347973
30	10	https://fcm.googleapis.com/fcm/send/eUKcLYNTOQ4:APA91bGZqeR_qcG6m3hbdvyglULFaUDBmxKxHsWU4CjTwxeoIL06X02AmnujAXvtWHu1obh58F_Sjv272Lms3KgviyqSl1SgrmynjEO3EWo3eQaYUWALgSTNikVyjKEoC8P5xZMqtBfq	BMyTnpVkDdnRgOFrQvZGVYVc5tCjVBhhlgFmtN+k6xrEB0exBPpG4W96/kmVXLWMNA37P2eurll0+PFp6o4y4GA=	1p5gB5JGsv7ve5Rhe6WUcA==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-09 18:40:14.858381	2026-08-09 18:40:14.858381
31	13	https://fcm.googleapis.com/fcm/send/dz8yyFZ-Luk:APA91bHQX3LZvVKT7p-bOnPJ1c0AHVuw_mCiJUGbC4OT9l6OTDPTN1U1xbNrQkr1URBXJHwggGg2xjW_zgYM8UhhUweyET72OrfZRpVpHiOg7bTyHgsVeMo9KZvkpUHUw7u5-yMqAIGc	BHW6z5IftZUvkQTA9Lhudi2Krg26lJ+LzdDrtMDsrFxnyNKQolr8iFuc/eofK3ZoF8sJ+Uvru84JZqsC3Mr82uA=	PxOg5zPWBw5LKhCji4e8Lw==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-09 18:49:36.660551	2026-08-09 18:49:36.660551
32	13	https://fcm.googleapis.com/fcm/send/dca9rFeGRf8:APA91bHSFZIZ7-MXi_XKTAoLqDu4PjRNCCtagO6bYig-1wNrv8c4s1dvMnFx3tR1XBsfdNSww4zvN4vvMXmhE4HFoaJTk7_YmcisOXs8X1asOq5tE6p0nv9Mwuj2lTFMItWlYtnRxcn5	BD1uCz9tLxuFgUQhjHzhw0iVc2oq1BtcqT1ruOyWsGMOQKuUy+s9Wv/A5PxIDMSz9ZBwJBh/1MJDyl3tvbcsKCg=	8OiI+T5VVTkQQ95DyXwfQw==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-09 19:23:36.27918	2026-08-09 19:23:36.279201
33	1	https://fcm.googleapis.com/fcm/send/dKaqYUrnrac:APA91bFmt9PRApMKxGPIXR7B_jJhslhlWQ5R3bgq30VONfLmM2-ZdvMgaHwzEbs2IB_V_4G7Awxjnbf0P9CCaTcDS2QgkyF_R44DCSDcI2wybJg2nQR60vVlDsD7HHIgsPNwgnV0lNmO	BPjiFRZsDGM8ZVWn1aT9b6spaRJEcb2KKJLKVGYU/tzQDrLUmja75qGHYXXv4ux8JD7EMCQOD3RFU9b6W1m7HaE=	P1QPzP32o1P+DOF5qL5W3w==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-09 19:24:16.512125	2026-08-09 19:24:16.512125
34	13	https://fcm.googleapis.com/fcm/send/df3xBoAgiwE:APA91bHgnlV3VzKUaOICSOE3a7x5AuvmrnHB0aSd5jPqIL8a3bJOfKvA7QsvstB80RVGTHoHeK4QJ0FQLRQpJ2hD0xZ-KmizBd-L42lxgrRQZQbEq8UvK871ywsdRrYuJPPf0KZ4YOAE	BFA9b+I7MdESq2FiIgXwoqLkqWg1aWrQpnHVH3gkioRmPrGckLeV8ZKtsWtdD07XkjS50U+W8ONmZ8U1syXCYYU=	P6Fbt59jex3sbd7WaQiYyw==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-09 19:29:07.013943	2026-08-09 19:29:07.013943
35	1	https://fcm.googleapis.com/fcm/send/eUTa0xnh-YY:APA91bEyARb0INsOklI_A99_ll1CdDbEFbT-g0HFiDL4adYZ_WGQw3D7Gr_2wVcZ4QE-bxVMRoqGlcMMnyNJHSl8Ih0tvBl0EcPLcbvfHEd0FDyHp4sY4uXwoAbd8C-0s6vYzOPLoWcl	BGo5v/5yooy6esV+vwQZVLX1g4AF/hUiGjjGZbqLYU5f7RkgoHaY/z1wRQWnX59zI4NTf821BhOqqUqsuEq058k=	aLCBF9BdYXLmI36uk7nUiQ==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-09 19:31:08.841032	2026-08-09 19:31:08.841032
36	13	https://fcm.googleapis.com/fcm/send/dNnqYqJRyGA:APA91bHgoFpDoL1NxamJXncHGigWYkmJtUxhREDMG6Zr-ltNnubMjA3TN0El5SczgfrXL-2J6XNkSocekn4oq2QwernaMJD8rloiwQ6f-vvjM83uln8wBepeGfn-lAU6HrEXwAVpmPTB	BDgipLxZ9dC1wSti51dVkaoLEg/qa5+WYFO3NM7usnPdEz7XHVnAbTQ5/LUXT/S+S2wbCBn4+G07GuLr5rL6iKw=	VDniQm8u3FZBb0lzj2Pk0g==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-13 01:29:12.378981	2026-08-13 01:29:12.379042
37	9	https://fcm.googleapis.com/fcm/send/db2HE15qDDE:APA91bH6ql8Qv6NMpNkJq1U1elSuqSzXOEC47d5AQj9lHP7Z6WQWvZwnjR0zbwmBnTVLGvHFCxNUSaAczoh2snO9Th299yEEpjZIGCcj0P6QjHCA7FTwWQ0TrTsIPKzghurQgSaMyZrt	BL4x1H4hjx1kpZOvBQt5Kd4WgfAFXlYYxTrgr0qiFIGzhzwyejT1sJhEV+kbmmemUl15QK6UQBeB4Epy5ovAYrc=	g0tNNRHuODlTlcfXSNf6Dw==	Mozilla/5.0 (Linux; Android 13; SM-G981B) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Mobile Safari/537.36	t	2026-08-13 01:38:15.788946	2026-08-13 01:38:15.788946
38	1	https://fcm.googleapis.com/fcm/send/d8FUP0pTmuQ:APA91bEZYnfCqprHkkiyEsJCkBOtMmqAF6WKwm8u-3E3awKT8GLpzjrFjOAA1sD6yS23XmKBeyY6TSK-OFGoO60Gf3Z0fEKeABYfjHbLraYfV54DegnNzRqZdZ1bj-wQ9-be4wVDwhkE	BECajxi/LJX0NKby/tX2dtXH3F4jlAoDR0FU7EqWFwB0v1Dzu16DElhMz/Okirrv8j5631LBUU9HTK93/zCfzZ0=	OfYaiGubFM9XWCCXZW6rpA==	Mozilla/5.0 (Linux; Android 13; SM-G981B) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Mobile Safari/537.36	t	2026-08-13 01:40:19.00174	2026-08-13 01:40:19.001741
39	15	https://fcm.googleapis.com/fcm/send/freosVWWMpE:APA91bFGiRFG4wd-Pv0UxhSecNTaKVtN2Q5x-lfhh0qpYYqsm0r47uzx4bcWEoLssC3u-eYXrPOcip8bZbmM63oshys2zseX9pTEe9GmbxYVumPejNhK9JwLUVhwMXfKDZaroA4SOzlc	BEYbfOv5xT+GgKCJTBI0F8Jhu6HnFxywwAKAA1ORMFDE6PCtLoBR1s0sBhBTM2IAjhDq7ABjgJ0lVtmkoRpr6fA=	hnNdF0JIbfJHb9hTUfY2Kg==	Mozilla/5.0 (Linux; Android 13; SM-G981B) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Mobile Safari/537.36	t	2026-08-13 01:41:11.876993	2026-08-13 01:41:11.876993
40	13	https://fcm.googleapis.com/fcm/send/cujdsAXqzYE:APA91bGUw4kF1CVuB458ULvxtDlzWHn0WFk5tvL1PUJoPcTwaXVuwcWeEw7_fQdTZYwsplF9tUf4rqpLH1SE1f05YtilMgPJVoZODRd5BvfEPhbbQWs_81TyRHwTWmB9QqP174EauaKw	BJ8ofkEtAbUAqiSjShjvCHzof1PQJNMq5Ax1Vl5AIBqWdAT5CBqTa/5Mi7HsR5J0RIIIPz/E9ao+xi+avJ59btY=	7bsUVoYIHP3lzerYmeQnDw==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-13 01:43:44.286167	2026-08-13 01:43:44.286167
41	9	https://fcm.googleapis.com/fcm/send/d7G5aYu20fA:APA91bHXOR6DqTyYlOqm9Fzr8SwElk7Dsf4fUQ-As5X7oaTC-xmyIhJFAYIKxYsPK_fE_gp2D9DfDzWw7f13qsbQ-TB_TSdUikda7Vl7eJLq8wsa9TAsDnc3lNDPT7gA2aV9XiI4hDbn	BF8MNHVoLwo2H3s/9yEriNn/ZusCrIX6as5dXHuVV4sCBHEoHNwcc3Yhh2TqpXPHsV3VeS3qTwGMYd34yqcMqDo=	eTryKsLatcVuTlrQWqJqGA==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-13 01:45:44.899821	2026-08-13 01:45:44.899821
42	1	https://fcm.googleapis.com/fcm/send/da1DRU5897Y:APA91bFw5W3M2XyQjF-5SgjO9nGnGufbrSl_OTEOwYbVqzKPhxOJyZrQgY6HTWObsojUAja3wvJKTYKYyf2Mkq4OsVzGJfYUiZNBnE1NdTMSspgJIifQ7niasq0ydV25xIYFvnCEFMLt	BIV7Outbwhe2/5p1XOUr94O4dD8lBU3LClFY1X4+wun1nve37rO/iTpzgJSsrzQgMmBdO5phbtRGcxYuPrwRy3o=	FTOif/HgUGzhsy/0jVPjwA==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-13 01:46:42.263302	2026-08-13 01:46:42.263302
43	16	https://fcm.googleapis.com/fcm/send/fiXNamHioT0:APA91bFpHgzGA1pAnIKZyK7OVhmc1VCmBrya-zhjs8IQMRrAoXl_6ZcabnXwmYcjVqmNfHN0ExuMjgmNfsRRxxuc5bq6gU-Xsq6ul-NmZVpZjHSMBV2ZCBrqUz7RmP2BSo3nmJkTqFxZ	BKvcIIxKqk/LlFDyz/V7CBHxzfC+wyfrnZBvYUezoq8GZB4ghk5BLDQumU1KalOLV+WNxYkEDl9mcpU+sINU4Ls=	qKpdudsBHs4v7qvdvQNFTg==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-13 01:47:14.750936	2026-08-13 01:47:14.750936
44	13	https://fcm.googleapis.com/fcm/send/fdDAYRVFknQ:APA91bEEI04Z0wn6yPxGycU7If84Ru_pxdKP_vDz0aV8dAPa9hFIo_wDGzHeEH2a07AuiVH-Y3XkxtlJfEOk0jmyqS9KWMjp4aKIabBVpl380cX47XW7w-mDvrSrScDbCRT2Bz6zvReL	BBag0kIZ5WTLl7bsmjJprWPDrPPhzZvZqPJnRj7a/rHvKrdnxdjYcPseNI+H2+OnfukvdPaFN0onYLWA+tlmOok=	3+fhkG9g0IFFUGWLoYwW0A==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-13 01:49:16.866084	2026-08-13 02:06:46.476562
45	13	https://fcm.googleapis.com/fcm/send/febSKg8frZk:APA91bE7TwbkCg7yDsXK63YwFwuSsDrOQOIaXy3av2GOIfby5hvA6pdVdvw-yBA339ZD39d7KIi-C-HruFNNxvrHhLfdfI3z8K3vxrnEonILh3pwJDoerQTeHOgWDCHEluEsYYGmS5Al	BGcXwepfBqeOlkL/SZr/7V/RU4PSG8wykkqXYKH0Q5d9lbVdNjJ1NZUea8kws9jofKVKumPweuJiu6BXvq42yS8=	kTgUKZIJXI6X681sUQlBjA==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-13 02:07:41.704969	2026-08-13 02:07:51.138183
46	1	https://fcm.googleapis.com/fcm/send/ccLctCA2oqk:APA91bHtr4yibJsqY3U4I0pwp4TjlJPG-u9mcNimgk5ScA2rd58SwH2Oc-WJpiTbRS_KC8hguZmhYkBZhRhOxC12ppf5yVcfP7u93DAzh18_JgjBWlAu4he6Ovd0Med5kFNF0vcmzqd2	BAFcFLn+PFh3kzq02I2KeDJD2UkceFYJ/qm2x+Z5yr+n72h2TpSOdE0ykUmhZEYzGkEg067ulvLIs6ZpH2FaV7Y=	AP7UlwORvH+Cg5wzgPEk2A==	Mozilla/5.0 (Linux; Android 13; SM-G981B) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Mobile Safari/537.36	t	2026-08-13 02:33:49.932651	2026-08-13 02:33:49.93273
47	1	https://fcm.googleapis.com/fcm/send/dua7mZkTnSk:APA91bG2W5pbPpLnUcNoZmSu4rCWe-S-6rUElQuoLhKMKZZ9lTYw3KCAR0RdQbcsDx38cb2ZLJOxvzsNz8kI93dAFpt-d1rVE6G7fv_cqpd7lUeJHcT-V_PRder01jlVEoLCfKlHUlWh	BH27Exs5j/gxCI5y7Wss4pAorZXfy/LA70GJtzNmT3rmaNi+luMjMZXx+X/oIX6++599O17/YtMRxpHnVeyGOTc=	SREYk/BX3Wws7phjTzS2ZA==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-13 02:54:45.752819	2026-08-13 02:54:45.753048
48	19	https://fcm.googleapis.com/fcm/send/fC7RAYngXFk:APA91bFhph1UiYqjpxbPtnUpRZuHlS07P8c3vWTpjEK9B8ybxne8vc8872NEI78BEvMpCK7CLwuP_2m53Y5E8RrLPB9yR8jkWBfNkyKLN-JYV-QoIp9v9CEerCvFINNmLmdtRj-2XLjY	BLDW0WDN89dFTSQE4NiT3/RXFDO+v0Tvohp9Kxm/TsBG8Y8VnQDJ5+tSREuDeBYq0+WompY5jFglXDAyqwGbj/8=	00fG2GaUDx20HgV9Ku8e6g==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-13 02:56:08.486235	2026-08-13 02:56:08.486235
49	20	https://fcm.googleapis.com/fcm/send/cKBCCdFKkx0:APA91bF6ucj_pAWbkpuEJbfrmEcblv5dTt3nBvz0TvEGOKnU5pJKviCUPyh6MdA4WK-Vju-jM_nGBK6jswhifLyUC3IhJEePbU3XPrhn9dnbswUQ5dQZr9silebvcsJXSVEUdtdiRI2u	BM0qIcc0jgacrZp6xx09cCSfvrVKEwtuz6seLF4TSabwzHFOX1AKcbDp/L1sBYTYe3qsWfjIRm9XEKiPoRKhyxY=	IC2Z1n6g3hW+ptZaPE3SMQ==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-13 02:57:38.900341	2026-08-13 02:57:38.900343
50	35	https://fcm.googleapis.com/fcm/send/eJBJQMSLDfU:APA91bHqiRVvzg3U15WKjoJHCAI_2ZIt4bMoK6iBTBy2NC_3e1beDWc0m_JOHrEACIDDpClIa6JOCYQy-h5xbCkbXncKhydGgWOYSv3aEYvaOhOzLInKnL0mzvUeyaLvnqpEU74IiOCX	BJAZ3di0EgvDgn9a0aoNXrOR1Rl2xuoFQmii3yiIUehveQ1v2Hm7HEFun4xKlA1WJDYaza3IUDT/PWBGHNnhDAk=	ggVlRGGY8t/N4XfQbynaHA==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-13 02:59:16.207243	2026-08-13 02:59:16.207243
51	15	https://fcm.googleapis.com/fcm/send/f5iyugPgoNg:APA91bGt4rUR0GbimparXQXq6KdzycgJa6z60TNKpTLjn4nLDQoq51yoLqN0hHB4M4GRsDgN8DNbbVGalOFTCJm9oKf4EvczL7RTZQp1FXh4j_2ELA4Nve9Xy6oaRG2YJh4tgBrGWMJf	BD9AUQ/PUpf4rxTkikAjfdcA0d+gu55+OZRtdG+xJlWDIh9QTmbyCJLsZbVlBht4UGlLwz+NjzhRLGeCXglBv04=	X+dkafV05PZhoZycFIhJhQ==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-13 03:13:48.490132	2026-08-13 03:13:48.490171
52	16	https://fcm.googleapis.com/fcm/send/fSnQnDlk95E:APA91bHMMUP8vt8wKTKnGY5gapOC8hSWC8R9pFgvjsFE7VnuX644P-X8H-OF5oDH3SoYirnsLNFhHFOGA9JV50lVa49NcYCpOqrblc2cHpFRAf31ZOoEgjzIvhCKfRySSMWhDDOyLVho	BCH0CdL6GrCIF0KVovyFMzFk+KHb9odAeRe/1tsPdbKG9Pb8kaCgwrDqwFOU2jcMoShacmbLOgaxcbGsfbz511Y=	bqGMinTmodqCFEdvPxFnSA==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-13 03:15:00.942846	2026-08-13 03:15:00.942846
53	19	https://fcm.googleapis.com/fcm/send/du_qZvrbbQk:APA91bHNSGbMKil8PVGz6CijvfsLW45zl5TlBqlQta0snQ2S5eT2htEm_IXszUuZSlDc46tMt3tk-KDjtecKC2lyhO2mfTHpaAQE2rQUHY5gWVakOnfaudcxDKmo9owtpbRcorDjCos2	BI90WPGCS16FWdaPbZmXJw1b42FVp3Nl5MUdoJWGiwjpxO+Hd/g9omM3jLxPjpr70c+eJ8b7S7zusmxtSuGCHoM=	OJQkiy/xDaP49yAFhNSIYg==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-13 03:15:49.592766	2026-08-13 03:15:49.592767
54	20	https://fcm.googleapis.com/fcm/send/dgDT6_9xqPs:APA91bGfbhF2ywpzNB2y_lAAx-g2eV1Kc9UsH6v-2EC4ksi4KEH8Xv136VEUxmcqs3Y7IYfhT6nRpTy_1GgwGHF6WskN1LTCfHo33xnst2WqbJVl9tfsU8rVVDo39Qyn6V00dEL54Jup	BPDZB33gWwQkymfQwQnXRtJNh97Q+qCi0uJj44cYHyK7JDVOvSLiR27BefY9iJjGXrPGYi7iQAf7lrf+nemG9mw=	O+H8nA4DoYV9gPW00hepxw==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-13 03:16:12.950765	2026-08-13 03:16:12.950765
55	1	https://fcm.googleapis.com/fcm/send/eFRd8AzuvOM:APA91bG_zWnGMHKBfRmT5xZO_b6RgpqTA_kjIAdg7mGpnpUC0eCXk30LCcodQWgjyrb5pK4meDzchJ_qryS3YcEK_abBMYJ0mhGKgIVWIHBSCe8LlHeaq8UfiNuLcv5t_mXZYy6meBw0	BHVvv7KUq8z4ii52Yy3RTiGiGf+u3LIpmJ9Fgu9Wo/dEW0TzEbv4CQPHglqdK9h3zpPOosc2FW1D5CGg+lJ9+5s=	/yi994kBQ0S+xXp3RRoGbA==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-13 21:16:46.608876	2026-08-13 21:16:46.608976
56	13	https://fcm.googleapis.com/fcm/send/en16cKFp7X4:APA91bFp8Zhw_5my2tW8ssBJp5rQA3Cp_3U--oRLYy64dYmFt2AlZP9Pg5EfujOA7UZIq0DNiygEShrjPWLd9PVE_Jtha_t5XO7th4L8xWXbiRemG-MUCydkeNdVq5tdktlpOOdTU4Fc	BEGWkTCPKrzi23oWVfzr0tHfF5oFi/cNmmUSmA5aGhxeCgvZVJ6bZiwfW/e3a6qAOXx+kh8afjl7BSxSLR1PMik=	wR1qJ6uZ7oWrIFI8Seakjw==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-13 21:31:55.368481	2026-08-13 21:31:55.368481
57	9	https://fcm.googleapis.com/fcm/send/fBwmIACRMdI:APA91bHZhqnCPXEIjtWRlvn-8HA41yyy00ZA3-oECJVKPEwDb8H2pg2DxrgZ2tclf9VmzMCogxf0DO7xfe2H2ZgOsX8KFMkzy-kjg6LsDaorwRhkcX51sxBgJ1gfV53Z6mV53VBB1ABP	BK2M2ETlVE9kOFqud34KrJCB280w+8dC7GH2qp/bie7AQGGA/Pe6Hml9UIrovk8JJhoWQsMbYosy8w9eD/P3hho=	kxnQkzR9EIXKfwbFrN7HEg==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-13 21:37:22.544778	2026-08-13 21:37:22.544778
58	13	https://fcm.googleapis.com/fcm/send/cPBrBLjKmnw:APA91bEsJjgPuCLRE72Hcy9p9OMJLUb4fSZRx-pCNpKsj722XBDH8ori3iLxLbRbZsiI8ngpSwyEaLDvi9Sqd3_3iwBpT0xcceHuTZJK7ecGPZ_e0tTdR2LwGS-NNcgghEoStmzyTZWS	BFkSR0HyCZXEmGZY+B/7MkjD2bx3GJwvI03qx4vAIDfbmlyJhvjkaHBaAQm4tRsF7Ev3DJJS7Fqfx+4IWMmsZ6E=	RzzO1PRdyfpjG9zy+wmL4w==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-13 21:38:38.147789	2026-08-13 21:38:38.147789
59	1	https://fcm.googleapis.com/fcm/send/cdyT10Hf6Xc:APA91bE3figkT64hJg9_MVAAz_sjShfYHvmQqvc6_SGiJ0n3sc8GZOKjOgLfpFzGXbKyA6RyCchApDCpK-dxqBAqn2oeP1ZZHYjUIGr7USw2A0e_wImDoRlUYohRZ5xN9asq6NYF4K_H	BA00bI+vxK4mAyHfSTaqRM/kcx1JrQUGTwaj5SjQCNynsbQh6020+Uu8EBaxUjacve+1sgBeDohlHSNUxPqhudg=	5DrIP85XvJ6eTP8NSXUx2w==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-15 19:46:02.448191	2026-08-15 19:46:02.44833
60	1	https://fcm.googleapis.com/fcm/send/dUZqJH0quDc:APA91bEG3qF9EvxdgdQqdZtZ9-e3ua_zwFFPynHx5J-O4ksyW3Snfbxz0TzsJb-wHQtcjTBDGrjpMK1Tq-PWbHU5M3DFsKc3fE-RJwR2e0hOVHoyVgieUSnjxdZgGQCcTZH0b7NCG1vk	BIQ6HtoEJqnAlca+R9Xxg9MJUDxYM4xRInzInfUs/9MYWp2dBhXhgAYkKK/dDAa4lThMo6c5OxzSbkdcjvoNhv0=	XW7np3RAa+DLVxzV/zRbog==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-20 18:47:56.810628	2026-08-20 18:47:56.810694
61	35	https://fcm.googleapis.com/fcm/send/f4JQnT4rDro:APA91bE10ZyBu5C6RAviEjaDFocIXSRN7eFXRiY0YWQ7Co1OUyjDANdjSNVoVxPDPwfYC7XphN3RUnRIOQ5ifBb4sd8Cc482FeNPhG4EX_hKvi4C3zZP2qz0OHo55Wns4QSzlb1ztR1n	BHMKX6yLzmJ02xOjTGtWB9rcTEPzLcUe2k4+/pidgvadig4jlbRG7gPbYAva9tu6c0GYT6b8wOEmqX//rfGSPEs=	dN/mQxBm986sfKpzJeETfg==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-20 18:48:41.999826	2026-08-20 18:48:41.999826
62	20	https://fcm.googleapis.com/fcm/send/dUbsn0OUfJY:APA91bGDD4y5t5a3e5q1yR-je7WsCsAhFlo1KGels61E1M6xRrTIUuzLUopCXGTggLvpseLTZT-PbHjAWB2Oc0sVGJPx1dVHOc8Vh6S4h6Q5OwekbJr4JQnI6eod7pTeLNy6vtpd0286	BMfyjp6TCn56tBSPClUqiLE1W/KIm+wrC0T5+nfKps/fRr8wuJABpjsuInXTST5vPdZAarhmRarG29a3fX1MZPU=	R51FegdceZBNKUatRF81qg==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-20 18:49:04.143168	2026-08-20 18:49:04.143168
63	19	https://fcm.googleapis.com/fcm/send/eQAp6NSMpZA:APA91bFOFRhC9AFTjoqIfC9IAUxtnTN_eUC1-P4beRVIz4WoMjDPZqJyNQcjHnxiUxZ2NIdFvNC1RIprUdumfUZoe5bffFNpFyD1icbvGsFFSDtb7TJoKXDHiukVjlQlqNAJqsbKXGWO	BFTo/lDJvWtvLfxRJi8HphL1aAldX6vRc9dRZYDaufikKCbKWPhDzkdCKuFYJ1FQWcJ5ZvcZmv7x/EIeTy4OuhQ=	OfrOk4rNC0a5TXnhjTtdyQ==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-20 18:53:35.808252	2026-08-20 18:53:35.808252
64	20	https://fcm.googleapis.com/fcm/send/dI7v6L1-pd0:APA91bE0ERaqAJdXnCznstDfp2D4qolyAnzYxl4M5Rz73D_1j6_EE0cQiK2BEyFMnVWx-7gTPEeEiw8vnV-y8iAiNACPIkub-l_KRUQl24C7dyS5DsRrviwQwcst3pFnAxZLhhH39whg	BBg02ZU1rm+Jx0vOMrp7EypAHI5AmYjFOE/cbajlTREY5EPIpS/wv8LrKlClIHn5WZG7dmYz4aAgrD9WHoenK2Y=	T2jk2sH+R8EnAYGdS8PzQA==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-20 19:06:17.41281	2026-08-20 19:06:17.412908
65	19	https://fcm.googleapis.com/fcm/send/cApnAex-1V4:APA91bHeQ3_5nW6G-a0tTt_x6v8Xgt86ASGJxpCOgTKPG9Lm6kCydlwqXyU72vmX0_0TlKQXCKe-izu3QaeZPcnNpq5SrHC5JKQfLHkLfY-Rmz9SEdqiucjhnA8TnSsujvF5DqBdgz0w	BMC06L3mhtCf2k0ORCf7b61bS3f2Y4xHzWzWvTW3vpxD1N9j1vBOVLAYFqQU7xJsDvxyPkXJjXDo2xwaJOun+vY=	/Yb7gaHSg2NAosBau9m87A==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-20 19:07:41.816915	2026-08-20 19:07:41.816916
66	1	https://fcm.googleapis.com/fcm/send/ff8JFQ2oo4I:APA91bGRp246IdH9qVGJJCIRqx8sh0Wztt84kwcNjWMmwUaBjuQ19En1hDdKpM_gZo8HYFfJrIcNbkYw8WaZCUXjZIZ2_VUYHAdach1JgbzU7trDIQLcIfSlrIZjkmknyQNb9jIwoKtj	BPl0VLiXtBnhx1ElpesZjtxw3f8rYw57L3P+t81EERCifPQh5KFRH4anHXbZXu7g/IZN6WFSMi0hLPmCV2x0MD0=	/lTaux6nXaAjrkAA3gyGbg==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-21 13:58:43.633183	2026-08-21 14:10:10.876517
67	1	https://fcm.googleapis.com/fcm/send/fPT3BxZmG4w:APA91bEuxNmfp0IW6_5zpDeaADXrUKJCM6bHR9_4ub_fQLLuvTAEvTezicNltJeF6vz2-DcVWsy5SHkeOvJObqWgYnhmgEcq_pNWO1JkUyHinTlAPb0qSB0EirPMrNnd4OCRMC4KirLc	BFi0lgmBbqJK9Marxnd+qB5w4iyNSk2yk/2/wT5F49Ef5S8jS/RPG2cXHXJCvRGBQirNSHBld5/0KzgnaRzMUIY=	s62z3CrrRkIuaeGYO4pQbw==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-21 16:43:17.212379	2026-08-21 16:43:17.212483
68	1	https://fcm.googleapis.com/fcm/send/cf9EzSQXrmE:APA91bHSZqZvNHkZU2IFz5yW1xSRIBdLbI9JpdPHBLE4E3sEXU_GxpdfaOB44Vpxufqm_XtuqjOgPnTStDW8-dTFjG68P3_II_c7Zxw-7zoJ_RpgC884bNgTlq7jeNzP_khxiE1ajmwP	BD0sl7PslbO4qSJZpK65qk/KupBgQOx51YGEMFq07XBo84qivXiIXGfH3aKdqJMMe7jDDkAN5tahoJLLxnXrCNw=	GTHpZl45BUh/HVudQCGlKw==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-21 16:57:21.713147	2026-08-21 16:58:50.806281
69	1	https://fcm.googleapis.com/fcm/send/fmOAidEV_mQ:APA91bEyySoi-t7LJ4YwxmkR8HYjVbv0IwWjTZ06zPA03jzezmMGjnSTtw8GgFDSfYbU-ylFZ6WR6oZTDQEe3FI3EnzRLtdO7LgDHFWxh_hQVMrqRQnlH5-Aph4wbEXkQq2oXmtv-LsC	BAK355Wu1jk33HsyrfXSk9134/ioMdxoapPWCzvGMim9Oc6aa0ccK8WxT9yclukmpQjgnqvG2eEWH80gCisiDWA=	YMpSvrLOrz3JmSHXQgLsXg==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-27 16:36:58.570097	2026-08-27 16:36:58.570171
70	1	https://fcm.googleapis.com/fcm/send/fbgHYHpeAmM:APA91bGxGpo4dezIM-e2a8X1a9wMCOT1UG8d_iaZC5JlmHzo01HO6pyMDiNM_ATlVVB3WeInpQzTzMUapAlU2aq_UlL_UA_1pHbpimv7j8WLho5JpyEN7aQqkuhzs7VQhKlJRSp0VBNm	BOfk7tB2nhCSIyi+H3xHCuXhFE4vF3X+8E84dKRih2aIqdXiH7FDtSRAMVPCYVyl+vx68u0ISjZFXaigLcOBEdY=	pDO8H7RdWkohi4UD/nzdnw==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-27 18:07:31.880535	2026-08-27 18:07:31.880893
71	9	https://fcm.googleapis.com/fcm/send/daZbJfROkb4:APA91bFXb841_jpx1LXAsA_-yySluvJgqp1Vusz5a_uOe8P7qLI7U9S60S7QLDfAy3S5KLiCdtAsvRyODts7I2i4pUMAaq5kPCTJvQrTB1JHrfgNmIyQQh7wcqTZCLbjGFsVypgWhIAd	BJNKlEHdof0eVTjMYuau0qSvLLKam0eeOM8AUHlcuSF/JiD+ON4fEjV6gTdL5M1zsyeCb/RZ2F3cwk3alLAVAus=	nPaWkaXoAnHzU7LZ29tqew==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-27 18:10:36.970147	2026-08-27 18:10:36.970147
72	13	https://fcm.googleapis.com/fcm/send/eaI6eC6zMnU:APA91bHDLyZauQxf3v69rPQjIzXxbs3eBkog1xDYPSXkyvQBoTutUBckNSpjrjlq3A7YelNnq9wyT4Yo456dwhqEXojl_HadBEOMz1ogvpu0OhPxXbHaYhYcGMI-mTIL4I5YyUd23hbN	BEZQFXVouFmgPlxHGNuhOjw/m7W7a9vBWf4eQC7pZi82XsyCGpGI0qSrVmyOKvRxBvN4cI0aGvyzoYKGBF2miPE=	6Xy3E0HUPYtXcZgVPt2TQg==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-27 18:13:56.664879	2026-08-27 18:13:56.664879
73	1	https://fcm.googleapis.com/fcm/send/eTDbXGexba0:APA91bEXPVpujKeGl7XH4LRiWBud-UUqzFMFWikL4Rf5fIwgxuiN-uokM9jti1ukeQr_eLxky5GuFeEe2g4qzeUU9GoT0f-tMCEl6m63VbCzif6JM3eGSY3ateymT8MFECuhWEGVRzOf	BOJlG++1Atclrx64LlDbszDEjCSW7tnMFrpP6FCx0Wx86X/4lIHRBxR9RHyLjtUBhqrjAjEjOjj9KluLVeY6Xr0=	s/SQSoH+dvUOorGl4sNUYQ==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-27 18:27:51.84205	2026-08-27 18:27:51.84205
74	19	https://fcm.googleapis.com/fcm/send/cUbai-GDK6E:APA91bFoUEhu5_vaujsAXhUwGn95B8mwYiwfZ-2Tf5CN2cE4ReFC2ik6XtdKbTxS0YbALZlHXnQ2nVLDEzyauIfsBaCimtXXz5T7Xq9rjm59l-_LkQii7b2mpHiVDiJlomoSns5UZSnD	BFTf7f9j6ZFrVrNHZ9ujJqjyrzKSugeZUKyiwfZ7uWkPaCRZOE1px5Z2/WA0GdQEuWO1q1gNkXH4fs7394I9XJo=	1ya+DJ4s8ZjHM8TVj1qD4g==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-27 18:28:37.610589	2026-08-27 18:28:37.610589
75	1	https://fcm.googleapis.com/fcm/send/c_SujY-bLmQ:APA91bGJ0wIdCn4jWHWughRU1fJW1ItXGnwpP6BL6UKSB4qm0TtRNwhH-GiLQKYKtuGtDjEBv9nZc5k7h5ITyiG4YcTizd1RoUnMgz1OlevzOgZRYEAigLYREoAP9qy1E6e88aPoN9H0	BOKj9fIGYv+bJ0GXJM9btzT5XnUcEWac0LsFP0k/aKe3MOPqsVDvOXoa/ltYIhOUoqEXU8ZT4eGBVKMCCJddhTo=	fHgayZqrp82VpwSaWL5aDg==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-27 18:38:16.80835	2026-08-27 18:38:16.80835
76	1	https://fcm.googleapis.com/fcm/send/dFkHHPetZyI:APA91bEInQKafmdJsFzMbIPE7zkP_A2oj7rehtwvrAdu2pbj4MZViwXYDKHmfi_u2gMhexkJbJ73ftCiXonPXoJjwfyzZwjayEvmPkVAUXEBKe5UFBw-Y3GUxhxE-YgTVtNP5TW05Pep	BPLW6+wfPfl8++DgCp8S3zDCrabkZ4WDjdKe1rtanwTa2aUMLa5SIwT2YS3LjGKtJnEEaAs2A7T/HFnffKKiqAQ=	4/1d0QsaWfZIUTvnZdzWtA==	Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/151.0.0.0 Safari/537.36	t	2026-08-28 14:39:16.276374	2026-08-28 14:39:16.276441
\.


--
-- Data for Name: __EFMigrationsHistory; Type: TABLE DATA; Schema: public; Owner: -
--

COPY public."__EFMigrationsHistory" ("MigrationId", "ProductVersion") FROM stdin;
20260828144557_InitialPostgres	8.0.0
\.


--
-- Name: Admissions_AdmissionID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."Admissions_AdmissionID_seq"', 4, true);


--
-- Name: Appointments_AppID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."Appointments_AppID_seq"', 22, true);


--
-- Name: Attachments_AttachmentID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."Attachments_AttachmentID_seq"', 2, true);


--
-- Name: AuditLogs_LogID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."AuditLogs_LogID_seq"', 392, true);


--
-- Name: Beds_BedID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."Beds_BedID_seq"', 6, true);


--
-- Name: ChartAccounts_AccountID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."ChartAccounts_AccountID_seq"', 26, true);


--
-- Name: CultureSensitivities_CultureSensitivityID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."CultureSensitivities_CultureSensitivityID_seq"', 1, true);


--
-- Name: CustomAssessmentTemplates_TemplateID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."CustomAssessmentTemplates_TemplateID_seq"', 5, true);


--
-- Name: DispenseRecords_DispenseID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."DispenseRecords_DispenseID_seq"', 4, true);


--
-- Name: DoctorCommissions_CommissionID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."DoctorCommissions_CommissionID_seq"', 4, true);


--
-- Name: DoctorProfiles_DoctorID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."DoctorProfiles_DoctorID_seq"', 9, true);


--
-- Name: EmployeeCourses_CourseID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."EmployeeCourses_CourseID_seq"', 1, true);


--
-- Name: EmployeeLeaves_LeaveID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."EmployeeLeaves_LeaveID_seq"', 5, true);


--
-- Name: EmployeeProfiles_EmployeeID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."EmployeeProfiles_EmployeeID_seq"', 5, true);


--
-- Name: HealthServices_ServiceID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."HealthServices_ServiceID_seq"', 3, true);


--
-- Name: InpatientCareExecutions_ExecutionID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."InpatientCareExecutions_ExecutionID_seq"', 3, true);


--
-- Name: InpatientCareOrders_OrderID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."InpatientCareOrders_OrderID_seq"', 4, true);


--
-- Name: InpatientDailyLogs_LogID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."InpatientDailyLogs_LogID_seq"', 5, true);


--
-- Name: InventoryCategories_CategoryID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."InventoryCategories_CategoryID_seq"', 1, true);


--
-- Name: InventoryItems_ItemID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."InventoryItems_ItemID_seq"', 1, true);


--
-- Name: Invoices_InvoiceID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."Invoices_InvoiceID_seq"', 9003, true);


--
-- Name: JournalEntries_JournalEntryID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."JournalEntries_JournalEntryID_seq"', 42, true);


--
-- Name: JournalEntryLines_JournalEntryLineID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."JournalEntryLines_JournalEntryLineID_seq"', 83, true);


--
-- Name: LabDevices_LabDeviceID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."LabDevices_LabDeviceID_seq"', 1, true);


--
-- Name: LabOrderItems_LabOrderItemID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."LabOrderItems_LabOrderItemID_seq"', 21, true);


--
-- Name: LabOrders_LabOrderID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."LabOrders_LabOrderID_seq"', 18, true);


--
-- Name: LabReferenceRanges_RangeID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."LabReferenceRanges_RangeID_seq"', 4, true);


--
-- Name: LabTests_LabTestID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."LabTests_LabTestID_seq"', 21, true);


--
-- Name: MedicalRecords_RecordID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."MedicalRecords_RecordID_seq"', 9, true);


--
-- Name: MedicationRequests_RequestID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."MedicationRequests_RequestID_seq"', 4, true);


--
-- Name: Medications_MedicationID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."Medications_MedicationID_seq"', 4, true);


--
-- Name: PatientAssessments_AssessmentID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."PatientAssessments_AssessmentID_seq"', 4, true);


--
-- Name: PatientProfiles_PatientID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."PatientProfiles_PatientID_seq"', 13, true);


--
-- Name: Prescriptions_PrescriptionID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."Prescriptions_PrescriptionID_seq"', 9, true);


--
-- Name: Priorities_PriorityID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."Priorities_PriorityID_seq"', 4, true);


--
-- Name: RadiologyOrders_RadiologyOrderID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."RadiologyOrders_RadiologyOrderID_seq"', 4, true);


--
-- Name: RadiologyTemplates_TemplateID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."RadiologyTemplates_TemplateID_seq"', 3, true);


--
-- Name: Rooms_RoomID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."Rooms_RoomID_seq"', 5, true);


--
-- Name: SalaryRecords_SalaryRecordID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."SalaryRecords_SalaryRecordID_seq"', 4, true);


--
-- Name: SensitivityResults_SensitivityResultID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."SensitivityResults_SensitivityResultID_seq"', 1, true);


--
-- Name: SoapNotes_SoapNoteID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."SoapNotes_SoapNoteID_seq"', 2, true);


--
-- Name: StockCountItems_StockCountItemID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."StockCountItems_StockCountItemID_seq"', 1, true);


--
-- Name: StockCounts_StockCountID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."StockCounts_StockCountID_seq"', 1, true);


--
-- Name: StockMovementItems_StockMovementItemID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."StockMovementItems_StockMovementItemID_seq"', 1, true);


--
-- Name: StockMovements_MovementID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."StockMovements_MovementID_seq"', 1, true);


--
-- Name: TelemedicineSessions_SessionID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."TelemedicineSessions_SessionID_seq"', 5, true);


--
-- Name: Treasuries_TreasuryID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."Treasuries_TreasuryID_seq"', 3, true);


--
-- Name: TriageQuestions_QuestionID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."TriageQuestions_QuestionID_seq"', 16, true);


--
-- Name: UserNotifications_NotificationID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."UserNotifications_NotificationID_seq"', 6, true);


--
-- Name: Users_UserID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."Users_UserID_seq"', 46, true);


--
-- Name: Vouchers_VoucherID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."Vouchers_VoucherID_seq"', 39, true);


--
-- Name: Wards_WardID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."Wards_WardID_seq"', 4, true);


--
-- Name: Warehouses_WarehouseID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."Warehouses_WarehouseID_seq"', 2, true);


--
-- Name: WebPushSubscriptions_SubscriptionID_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public."WebPushSubscriptions_SubscriptionID_seq"', 77, true);


--
-- Name: Admissions PK_Admissions; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Admissions"
    ADD CONSTRAINT "PK_Admissions" PRIMARY KEY ("AdmissionID");


--
-- Name: Appointments PK_Appointments; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Appointments"
    ADD CONSTRAINT "PK_Appointments" PRIMARY KEY ("AppID");


--
-- Name: Attachments PK_Attachments; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Attachments"
    ADD CONSTRAINT "PK_Attachments" PRIMARY KEY ("AttachmentID");


--
-- Name: AuditLogs PK_AuditLogs; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AuditLogs"
    ADD CONSTRAINT "PK_AuditLogs" PRIMARY KEY ("LogID");


--
-- Name: Beds PK_Beds; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Beds"
    ADD CONSTRAINT "PK_Beds" PRIMARY KEY ("BedID");


--
-- Name: ChartAccounts PK_ChartAccounts; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ChartAccounts"
    ADD CONSTRAINT "PK_ChartAccounts" PRIMARY KEY ("AccountID");


--
-- Name: CultureSensitivities PK_CultureSensitivities; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CultureSensitivities"
    ADD CONSTRAINT "PK_CultureSensitivities" PRIMARY KEY ("CultureSensitivityID");


--
-- Name: CustomAssessmentTemplates PK_CustomAssessmentTemplates; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CustomAssessmentTemplates"
    ADD CONSTRAINT "PK_CustomAssessmentTemplates" PRIMARY KEY ("TemplateID");


--
-- Name: DispenseRecords PK_DispenseRecords; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DispenseRecords"
    ADD CONSTRAINT "PK_DispenseRecords" PRIMARY KEY ("DispenseID");


--
-- Name: DoctorCommissions PK_DoctorCommissions; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DoctorCommissions"
    ADD CONSTRAINT "PK_DoctorCommissions" PRIMARY KEY ("CommissionID");


--
-- Name: DoctorProfiles PK_DoctorProfiles; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DoctorProfiles"
    ADD CONSTRAINT "PK_DoctorProfiles" PRIMARY KEY ("DoctorID");


--
-- Name: EmployeeCourses PK_EmployeeCourses; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EmployeeCourses"
    ADD CONSTRAINT "PK_EmployeeCourses" PRIMARY KEY ("CourseID");


--
-- Name: EmployeeLeaves PK_EmployeeLeaves; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EmployeeLeaves"
    ADD CONSTRAINT "PK_EmployeeLeaves" PRIMARY KEY ("LeaveID");


--
-- Name: EmployeeProfiles PK_EmployeeProfiles; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EmployeeProfiles"
    ADD CONSTRAINT "PK_EmployeeProfiles" PRIMARY KEY ("EmployeeID");


--
-- Name: HealthServices PK_HealthServices; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."HealthServices"
    ADD CONSTRAINT "PK_HealthServices" PRIMARY KEY ("ServiceID");


--
-- Name: InpatientCareExecutions PK_InpatientCareExecutions; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InpatientCareExecutions"
    ADD CONSTRAINT "PK_InpatientCareExecutions" PRIMARY KEY ("ExecutionID");


--
-- Name: InpatientCareOrders PK_InpatientCareOrders; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InpatientCareOrders"
    ADD CONSTRAINT "PK_InpatientCareOrders" PRIMARY KEY ("OrderID");


--
-- Name: InpatientDailyLogs PK_InpatientDailyLogs; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InpatientDailyLogs"
    ADD CONSTRAINT "PK_InpatientDailyLogs" PRIMARY KEY ("LogID");


--
-- Name: InventoryCategories PK_InventoryCategories; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InventoryCategories"
    ADD CONSTRAINT "PK_InventoryCategories" PRIMARY KEY ("CategoryID");


--
-- Name: InventoryItems PK_InventoryItems; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InventoryItems"
    ADD CONSTRAINT "PK_InventoryItems" PRIMARY KEY ("ItemID");


--
-- Name: Invoices PK_Invoices; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Invoices"
    ADD CONSTRAINT "PK_Invoices" PRIMARY KEY ("InvoiceID");


--
-- Name: JournalEntries PK_JournalEntries; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."JournalEntries"
    ADD CONSTRAINT "PK_JournalEntries" PRIMARY KEY ("JournalEntryID");


--
-- Name: JournalEntryLines PK_JournalEntryLines; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."JournalEntryLines"
    ADD CONSTRAINT "PK_JournalEntryLines" PRIMARY KEY ("JournalEntryLineID");


--
-- Name: LabDevices PK_LabDevices; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."LabDevices"
    ADD CONSTRAINT "PK_LabDevices" PRIMARY KEY ("LabDeviceID");


--
-- Name: LabOrderItems PK_LabOrderItems; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."LabOrderItems"
    ADD CONSTRAINT "PK_LabOrderItems" PRIMARY KEY ("LabOrderItemID");


--
-- Name: LabOrders PK_LabOrders; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."LabOrders"
    ADD CONSTRAINT "PK_LabOrders" PRIMARY KEY ("LabOrderID");


--
-- Name: LabReferenceRanges PK_LabReferenceRanges; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."LabReferenceRanges"
    ADD CONSTRAINT "PK_LabReferenceRanges" PRIMARY KEY ("RangeID");


--
-- Name: LabTests PK_LabTests; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."LabTests"
    ADD CONSTRAINT "PK_LabTests" PRIMARY KEY ("LabTestID");


--
-- Name: MedicalRecords PK_MedicalRecords; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."MedicalRecords"
    ADD CONSTRAINT "PK_MedicalRecords" PRIMARY KEY ("RecordID");


--
-- Name: MedicationRequests PK_MedicationRequests; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."MedicationRequests"
    ADD CONSTRAINT "PK_MedicationRequests" PRIMARY KEY ("RequestID");


--
-- Name: Medications PK_Medications; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Medications"
    ADD CONSTRAINT "PK_Medications" PRIMARY KEY ("MedicationID");


--
-- Name: PatientAssessments PK_PatientAssessments; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PatientAssessments"
    ADD CONSTRAINT "PK_PatientAssessments" PRIMARY KEY ("AssessmentID");


--
-- Name: PatientProfiles PK_PatientProfiles; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PatientProfiles"
    ADD CONSTRAINT "PK_PatientProfiles" PRIMARY KEY ("PatientID");


--
-- Name: Prescriptions PK_Prescriptions; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Prescriptions"
    ADD CONSTRAINT "PK_Prescriptions" PRIMARY KEY ("PrescriptionID");


--
-- Name: Priorities PK_Priorities; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Priorities"
    ADD CONSTRAINT "PK_Priorities" PRIMARY KEY ("PriorityID");


--
-- Name: PsychiatricRecords PK_PsychiatricRecords; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PsychiatricRecords"
    ADD CONSTRAINT "PK_PsychiatricRecords" PRIMARY KEY ("RecordID");


--
-- Name: RadiologyOrders PK_RadiologyOrders; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."RadiologyOrders"
    ADD CONSTRAINT "PK_RadiologyOrders" PRIMARY KEY ("RadiologyOrderID");


--
-- Name: RadiologyTemplates PK_RadiologyTemplates; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."RadiologyTemplates"
    ADD CONSTRAINT "PK_RadiologyTemplates" PRIMARY KEY ("TemplateID");


--
-- Name: Rooms PK_Rooms; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Rooms"
    ADD CONSTRAINT "PK_Rooms" PRIMARY KEY ("RoomID");


--
-- Name: SalaryRecords PK_SalaryRecords; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SalaryRecords"
    ADD CONSTRAINT "PK_SalaryRecords" PRIMARY KEY ("SalaryRecordID");


--
-- Name: SensitivityResults PK_SensitivityResults; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SensitivityResults"
    ADD CONSTRAINT "PK_SensitivityResults" PRIMARY KEY ("SensitivityResultID");


--
-- Name: SoapNotes PK_SoapNotes; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SoapNotes"
    ADD CONSTRAINT "PK_SoapNotes" PRIMARY KEY ("SoapNoteID");


--
-- Name: StockCountItems PK_StockCountItems; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockCountItems"
    ADD CONSTRAINT "PK_StockCountItems" PRIMARY KEY ("StockCountItemID");


--
-- Name: StockCounts PK_StockCounts; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockCounts"
    ADD CONSTRAINT "PK_StockCounts" PRIMARY KEY ("StockCountID");


--
-- Name: StockMovementItems PK_StockMovementItems; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockMovementItems"
    ADD CONSTRAINT "PK_StockMovementItems" PRIMARY KEY ("StockMovementItemID");


--
-- Name: StockMovements PK_StockMovements; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockMovements"
    ADD CONSTRAINT "PK_StockMovements" PRIMARY KEY ("MovementID");


--
-- Name: SystemSettings PK_SystemSettings; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SystemSettings"
    ADD CONSTRAINT "PK_SystemSettings" PRIMARY KEY ("SettingKey");


--
-- Name: TelemedicineSessions PK_TelemedicineSessions; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."TelemedicineSessions"
    ADD CONSTRAINT "PK_TelemedicineSessions" PRIMARY KEY ("SessionID");


--
-- Name: Treasuries PK_Treasuries; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Treasuries"
    ADD CONSTRAINT "PK_Treasuries" PRIMARY KEY ("TreasuryID");


--
-- Name: TriageQuestions PK_TriageQuestions; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."TriageQuestions"
    ADD CONSTRAINT "PK_TriageQuestions" PRIMARY KEY ("QuestionID");


--
-- Name: UserNotifications PK_UserNotifications; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserNotifications"
    ADD CONSTRAINT "PK_UserNotifications" PRIMARY KEY ("NotificationID");


--
-- Name: Users PK_Users; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "PK_Users" PRIMARY KEY ("UserID");


--
-- Name: Vouchers PK_Vouchers; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Vouchers"
    ADD CONSTRAINT "PK_Vouchers" PRIMARY KEY ("VoucherID");


--
-- Name: Wards PK_Wards; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Wards"
    ADD CONSTRAINT "PK_Wards" PRIMARY KEY ("WardID");


--
-- Name: Warehouses PK_Warehouses; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Warehouses"
    ADD CONSTRAINT "PK_Warehouses" PRIMARY KEY ("WarehouseID");


--
-- Name: WebPushSubscriptions PK_WebPushSubscriptions; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."WebPushSubscriptions"
    ADD CONSTRAINT "PK_WebPushSubscriptions" PRIMARY KEY ("SubscriptionID");


--
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- Name: IX_Admissions_BedID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Admissions_BedID" ON public."Admissions" USING btree ("BedID");


--
-- Name: IX_Admissions_DoctorID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Admissions_DoctorID" ON public."Admissions" USING btree ("DoctorID");


--
-- Name: IX_Admissions_PatientID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Admissions_PatientID" ON public."Admissions" USING btree ("PatientID");


--
-- Name: IX_Appointments_DoctorID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Appointments_DoctorID" ON public."Appointments" USING btree ("DoctorID");


--
-- Name: IX_Appointments_PatientID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Appointments_PatientID" ON public."Appointments" USING btree ("PatientID");


--
-- Name: IX_Appointments_PriorityID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Appointments_PriorityID" ON public."Appointments" USING btree ("PriorityID");


--
-- Name: IX_Attachments_PatientID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Attachments_PatientID" ON public."Attachments" USING btree ("PatientID");


--
-- Name: IX_Attachments_RecordID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Attachments_RecordID" ON public."Attachments" USING btree ("RecordID");


--
-- Name: IX_AuditLogs_UserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_AuditLogs_UserID" ON public."AuditLogs" USING btree ("UserID");


--
-- Name: IX_Beds_RoomID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Beds_RoomID" ON public."Beds" USING btree ("RoomID");


--
-- Name: IX_ChartAccounts_AccountCode; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_ChartAccounts_AccountCode" ON public."ChartAccounts" USING btree ("AccountCode");


--
-- Name: IX_ChartAccounts_ParentAccountID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_ChartAccounts_ParentAccountID" ON public."ChartAccounts" USING btree ("ParentAccountID");


--
-- Name: IX_CultureSensitivities_LabOrderItemID; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_CultureSensitivities_LabOrderItemID" ON public."CultureSensitivities" USING btree ("LabOrderItemID");


--
-- Name: IX_CustomAssessmentTemplates_DoctorID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_CustomAssessmentTemplates_DoctorID" ON public."CustomAssessmentTemplates" USING btree ("DoctorID");


--
-- Name: IX_DispenseRecords_DispensedByUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_DispenseRecords_DispensedByUserID" ON public."DispenseRecords" USING btree ("DispensedByUserID");


--
-- Name: IX_DispenseRecords_MedicationID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_DispenseRecords_MedicationID" ON public."DispenseRecords" USING btree ("MedicationID");


--
-- Name: IX_DispenseRecords_PrescriptionID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_DispenseRecords_PrescriptionID" ON public."DispenseRecords" USING btree ("PrescriptionID");


--
-- Name: IX_DoctorCommissions_DoctorID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_DoctorCommissions_DoctorID" ON public."DoctorCommissions" USING btree ("DoctorID");


--
-- Name: IX_DoctorProfiles_UserID; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_DoctorProfiles_UserID" ON public."DoctorProfiles" USING btree ("UserID");


--
-- Name: IX_EmployeeCourses_EmployeeID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_EmployeeCourses_EmployeeID" ON public."EmployeeCourses" USING btree ("EmployeeID");


--
-- Name: IX_EmployeeLeaves_ApprovedByUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_EmployeeLeaves_ApprovedByUserID" ON public."EmployeeLeaves" USING btree ("ApprovedByUserID");


--
-- Name: IX_EmployeeLeaves_EmployeeID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_EmployeeLeaves_EmployeeID" ON public."EmployeeLeaves" USING btree ("EmployeeID");


--
-- Name: IX_EmployeeProfiles_EmployeeNumber; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_EmployeeProfiles_EmployeeNumber" ON public."EmployeeProfiles" USING btree ("EmployeeNumber");


--
-- Name: IX_EmployeeProfiles_UserID; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_EmployeeProfiles_UserID" ON public."EmployeeProfiles" USING btree ("UserID");


--
-- Name: IX_InpatientCareExecutions_ExecutedByUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_InpatientCareExecutions_ExecutedByUserID" ON public."InpatientCareExecutions" USING btree ("ExecutedByUserID");


--
-- Name: IX_InpatientCareExecutions_OrderID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_InpatientCareExecutions_OrderID" ON public."InpatientCareExecutions" USING btree ("OrderID");


--
-- Name: IX_InpatientCareOrders_AdmissionID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_InpatientCareOrders_AdmissionID" ON public."InpatientCareOrders" USING btree ("AdmissionID");


--
-- Name: IX_InpatientCareOrders_CreatedByUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_InpatientCareOrders_CreatedByUserID" ON public."InpatientCareOrders" USING btree ("CreatedByUserID");


--
-- Name: IX_InpatientCareOrders_HealthServiceID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_InpatientCareOrders_HealthServiceID" ON public."InpatientCareOrders" USING btree ("HealthServiceID");


--
-- Name: IX_InpatientDailyLogs_AdmissionID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_InpatientDailyLogs_AdmissionID" ON public."InpatientDailyLogs" USING btree ("AdmissionID");


--
-- Name: IX_InpatientDailyLogs_LoggedByUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_InpatientDailyLogs_LoggedByUserID" ON public."InpatientDailyLogs" USING btree ("LoggedByUserID");


--
-- Name: IX_InventoryCategories_ParentCategoryID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_InventoryCategories_ParentCategoryID" ON public."InventoryCategories" USING btree ("ParentCategoryID");


--
-- Name: IX_InventoryItems_CategoryID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_InventoryItems_CategoryID" ON public."InventoryItems" USING btree ("CategoryID");


--
-- Name: IX_InventoryItems_ItemCode; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_InventoryItems_ItemCode" ON public."InventoryItems" USING btree ("ItemCode");


--
-- Name: IX_InventoryItems_MedicationID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_InventoryItems_MedicationID" ON public."InventoryItems" USING btree ("MedicationID");


--
-- Name: IX_Invoices_AppointmentID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Invoices_AppointmentID" ON public."Invoices" USING btree ("AppointmentID");


--
-- Name: IX_Invoices_DispenseRecordID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Invoices_DispenseRecordID" ON public."Invoices" USING btree ("DispenseRecordID");


--
-- Name: IX_Invoices_DoctorCommissionID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Invoices_DoctorCommissionID" ON public."Invoices" USING btree ("DoctorCommissionID");


--
-- Name: IX_Invoices_DoctorID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Invoices_DoctorID" ON public."Invoices" USING btree ("DoctorID");


--
-- Name: IX_Invoices_LabOrderID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Invoices_LabOrderID" ON public."Invoices" USING btree ("LabOrderID");


--
-- Name: IX_Invoices_PatientUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Invoices_PatientUserID" ON public."Invoices" USING btree ("PatientUserID");


--
-- Name: IX_Invoices_RadiologyOrderID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Invoices_RadiologyOrderID" ON public."Invoices" USING btree ("RadiologyOrderID");


--
-- Name: IX_JournalEntries_CreatedByUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_JournalEntries_CreatedByUserID" ON public."JournalEntries" USING btree ("CreatedByUserID");


--
-- Name: IX_JournalEntries_EntryNumber; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_JournalEntries_EntryNumber" ON public."JournalEntries" USING btree ("EntryNumber");


--
-- Name: IX_JournalEntries_PostedByUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_JournalEntries_PostedByUserID" ON public."JournalEntries" USING btree ("PostedByUserID");


--
-- Name: IX_JournalEntryLines_AccountID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_JournalEntryLines_AccountID" ON public."JournalEntryLines" USING btree ("AccountID");


--
-- Name: IX_JournalEntryLines_JournalEntryID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_JournalEntryLines_JournalEntryID" ON public."JournalEntryLines" USING btree ("JournalEntryID");


--
-- Name: IX_LabDevices_DeviceCode; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_LabDevices_DeviceCode" ON public."LabDevices" USING btree ("DeviceCode");


--
-- Name: IX_LabOrderItems_LabOrderID_LabTestID; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_LabOrderItems_LabOrderID_LabTestID" ON public."LabOrderItems" USING btree ("LabOrderID", "LabTestID");


--
-- Name: IX_LabOrderItems_LabTestID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_LabOrderItems_LabTestID" ON public."LabOrderItems" USING btree ("LabTestID");


--
-- Name: IX_LabOrders_DoctorID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_LabOrders_DoctorID" ON public."LabOrders" USING btree ("DoctorID");


--
-- Name: IX_LabOrders_LabTestID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_LabOrders_LabTestID" ON public."LabOrders" USING btree ("LabTestID");


--
-- Name: IX_LabOrders_PatientUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_LabOrders_PatientUserID" ON public."LabOrders" USING btree ("PatientUserID");


--
-- Name: IX_LabReferenceRanges_LabTestID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_LabReferenceRanges_LabTestID" ON public."LabReferenceRanges" USING btree ("LabTestID");


--
-- Name: IX_LabTests_DeviceID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_LabTests_DeviceID" ON public."LabTests" USING btree ("DeviceID");


--
-- Name: IX_LabTests_PanelID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_LabTests_PanelID" ON public."LabTests" USING btree ("PanelID");


--
-- Name: IX_MedicalRecords_AppID; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_MedicalRecords_AppID" ON public."MedicalRecords" USING btree ("AppID");


--
-- Name: IX_MedicationRequests_DoctorUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_MedicationRequests_DoctorUserID" ON public."MedicationRequests" USING btree ("DoctorUserID");


--
-- Name: IX_PatientAssessments_PatientUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_PatientAssessments_PatientUserID" ON public."PatientAssessments" USING btree ("PatientUserID");


--
-- Name: IX_PatientAssessments_TemplateID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_PatientAssessments_TemplateID" ON public."PatientAssessments" USING btree ("TemplateID");


--
-- Name: IX_PatientProfiles_FileNumber; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_PatientProfiles_FileNumber" ON public."PatientProfiles" USING btree ("FileNumber");


--
-- Name: IX_PatientProfiles_UserID; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_PatientProfiles_UserID" ON public."PatientProfiles" USING btree ("UserID");


--
-- Name: IX_Prescriptions_MedicationID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Prescriptions_MedicationID" ON public."Prescriptions" USING btree ("MedicationID");


--
-- Name: IX_Prescriptions_RecordID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Prescriptions_RecordID" ON public."Prescriptions" USING btree ("RecordID");


--
-- Name: IX_RadiologyOrders_DoctorID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_RadiologyOrders_DoctorID" ON public."RadiologyOrders" USING btree ("DoctorID");


--
-- Name: IX_RadiologyOrders_PatientUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_RadiologyOrders_PatientUserID" ON public."RadiologyOrders" USING btree ("PatientUserID");


--
-- Name: IX_RadiologyOrders_RadiologistID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_RadiologyOrders_RadiologistID" ON public."RadiologyOrders" USING btree ("RadiologistID");


--
-- Name: IX_Rooms_WardID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Rooms_WardID" ON public."Rooms" USING btree ("WardID");


--
-- Name: IX_SalaryRecords_CreatedByUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_SalaryRecords_CreatedByUserID" ON public."SalaryRecords" USING btree ("CreatedByUserID");


--
-- Name: IX_SalaryRecords_EmployeeID_PeriodYear_PeriodMonth; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_SalaryRecords_EmployeeID_PeriodYear_PeriodMonth" ON public."SalaryRecords" USING btree ("EmployeeID", "PeriodYear", "PeriodMonth");


--
-- Name: IX_SalaryRecords_JournalEntryID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_SalaryRecords_JournalEntryID" ON public."SalaryRecords" USING btree ("JournalEntryID");


--
-- Name: IX_SensitivityResults_CultureSensitivityID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_SensitivityResults_CultureSensitivityID" ON public."SensitivityResults" USING btree ("CultureSensitivityID");


--
-- Name: IX_SoapNotes_RecordID; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_SoapNotes_RecordID" ON public."SoapNotes" USING btree ("RecordID");


--
-- Name: IX_StockCountItems_ItemID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_StockCountItems_ItemID" ON public."StockCountItems" USING btree ("ItemID");


--
-- Name: IX_StockCountItems_StockCountID_ItemID; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_StockCountItems_StockCountID_ItemID" ON public."StockCountItems" USING btree ("StockCountID", "ItemID");


--
-- Name: IX_StockCounts_CreatedByUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_StockCounts_CreatedByUserID" ON public."StockCounts" USING btree ("CreatedByUserID");


--
-- Name: IX_StockCounts_PostedByUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_StockCounts_PostedByUserID" ON public."StockCounts" USING btree ("PostedByUserID");


--
-- Name: IX_StockCounts_ReversedByUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_StockCounts_ReversedByUserID" ON public."StockCounts" USING btree ("ReversedByUserID");


--
-- Name: IX_StockCounts_StockCountNumber; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_StockCounts_StockCountNumber" ON public."StockCounts" USING btree ("StockCountNumber");


--
-- Name: IX_StockCounts_WarehouseID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_StockCounts_WarehouseID" ON public."StockCounts" USING btree ("WarehouseID");


--
-- Name: IX_StockMovementItems_ItemID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_StockMovementItems_ItemID" ON public."StockMovementItems" USING btree ("ItemID");


--
-- Name: IX_StockMovementItems_MovementID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_StockMovementItems_MovementID" ON public."StockMovementItems" USING btree ("MovementID");


--
-- Name: IX_StockMovements_CreatedByUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_StockMovements_CreatedByUserID" ON public."StockMovements" USING btree ("CreatedByUserID");


--
-- Name: IX_StockMovements_MovementNumber; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_StockMovements_MovementNumber" ON public."StockMovements" USING btree ("MovementNumber");


--
-- Name: IX_StockMovements_PostedByUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_StockMovements_PostedByUserID" ON public."StockMovements" USING btree ("PostedByUserID");


--
-- Name: IX_StockMovements_ToWarehouseID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_StockMovements_ToWarehouseID" ON public."StockMovements" USING btree ("ToWarehouseID");


--
-- Name: IX_StockMovements_WarehouseID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_StockMovements_WarehouseID" ON public."StockMovements" USING btree ("WarehouseID");


--
-- Name: IX_TelemedicineSessions_AppointmentID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_TelemedicineSessions_AppointmentID" ON public."TelemedicineSessions" USING btree ("AppointmentID");


--
-- Name: IX_TelemedicineSessions_RoomCode; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_TelemedicineSessions_RoomCode" ON public."TelemedicineSessions" USING btree ("RoomCode");


--
-- Name: IX_Treasuries_AccountID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Treasuries_AccountID" ON public."Treasuries" USING btree ("AccountID");


--
-- Name: IX_Treasuries_TreasuryCode; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Treasuries_TreasuryCode" ON public."Treasuries" USING btree ("TreasuryCode");


--
-- Name: IX_UserNotifications_UserID_IsRead; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_UserNotifications_UserID_IsRead" ON public."UserNotifications" USING btree ("UserID", "IsRead");


--
-- Name: IX_Users_AssignedTreasuryID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Users_AssignedTreasuryID" ON public."Users" USING btree ("AssignedTreasuryID");


--
-- Name: IX_Users_Email; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Users_Email" ON public."Users" USING btree ("Email");


--
-- Name: IX_Vouchers_AccountID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Vouchers_AccountID" ON public."Vouchers" USING btree ("AccountID");


--
-- Name: IX_Vouchers_AppointmentID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Vouchers_AppointmentID" ON public."Vouchers" USING btree ("AppointmentID");


--
-- Name: IX_Vouchers_CreatedByUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Vouchers_CreatedByUserID" ON public."Vouchers" USING btree ("CreatedByUserID");


--
-- Name: IX_Vouchers_InvoiceID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Vouchers_InvoiceID" ON public."Vouchers" USING btree ("InvoiceID");


--
-- Name: IX_Vouchers_PatientUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Vouchers_PatientUserID" ON public."Vouchers" USING btree ("PatientUserID");


--
-- Name: IX_Vouchers_PostedByUserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Vouchers_PostedByUserID" ON public."Vouchers" USING btree ("PostedByUserID");


--
-- Name: IX_Vouchers_ToTreasuryID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Vouchers_ToTreasuryID" ON public."Vouchers" USING btree ("ToTreasuryID");


--
-- Name: IX_Vouchers_TreasuryID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_Vouchers_TreasuryID" ON public."Vouchers" USING btree ("TreasuryID");


--
-- Name: IX_Vouchers_VoucherNumber; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Vouchers_VoucherNumber" ON public."Vouchers" USING btree ("VoucherNumber");


--
-- Name: IX_Warehouses_WarehouseCode; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_Warehouses_WarehouseCode" ON public."Warehouses" USING btree ("WarehouseCode");


--
-- Name: IX_WebPushSubscriptions_Endpoint; Type: INDEX; Schema: public; Owner: -
--

CREATE UNIQUE INDEX "IX_WebPushSubscriptions_Endpoint" ON public."WebPushSubscriptions" USING btree ("Endpoint");


--
-- Name: IX_WebPushSubscriptions_UserID; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX "IX_WebPushSubscriptions_UserID" ON public."WebPushSubscriptions" USING btree ("UserID");


--
-- Name: Admissions FK_Admissions_Beds_BedID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Admissions"
    ADD CONSTRAINT "FK_Admissions_Beds_BedID" FOREIGN KEY ("BedID") REFERENCES public."Beds"("BedID") ON DELETE RESTRICT;


--
-- Name: Admissions FK_Admissions_DoctorProfiles_DoctorID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Admissions"
    ADD CONSTRAINT "FK_Admissions_DoctorProfiles_DoctorID" FOREIGN KEY ("DoctorID") REFERENCES public."DoctorProfiles"("DoctorID") ON DELETE RESTRICT;


--
-- Name: Admissions FK_Admissions_PatientProfiles_PatientID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Admissions"
    ADD CONSTRAINT "FK_Admissions_PatientProfiles_PatientID" FOREIGN KEY ("PatientID") REFERENCES public."PatientProfiles"("PatientID") ON DELETE RESTRICT;


--
-- Name: Appointments FK_Appointments_DoctorProfiles_DoctorID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Appointments"
    ADD CONSTRAINT "FK_Appointments_DoctorProfiles_DoctorID" FOREIGN KEY ("DoctorID") REFERENCES public."DoctorProfiles"("DoctorID") ON DELETE RESTRICT;


--
-- Name: Appointments FK_Appointments_PatientProfiles_PatientID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Appointments"
    ADD CONSTRAINT "FK_Appointments_PatientProfiles_PatientID" FOREIGN KEY ("PatientID") REFERENCES public."PatientProfiles"("PatientID") ON DELETE RESTRICT;


--
-- Name: Appointments FK_Appointments_Priorities_PriorityID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Appointments"
    ADD CONSTRAINT "FK_Appointments_Priorities_PriorityID" FOREIGN KEY ("PriorityID") REFERENCES public."Priorities"("PriorityID") ON DELETE RESTRICT;


--
-- Name: Attachments FK_Attachments_MedicalRecords_RecordID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Attachments"
    ADD CONSTRAINT "FK_Attachments_MedicalRecords_RecordID" FOREIGN KEY ("RecordID") REFERENCES public."MedicalRecords"("RecordID") ON DELETE SET NULL;


--
-- Name: Attachments FK_Attachments_PatientProfiles_PatientID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Attachments"
    ADD CONSTRAINT "FK_Attachments_PatientProfiles_PatientID" FOREIGN KEY ("PatientID") REFERENCES public."PatientProfiles"("PatientID") ON DELETE SET NULL;


--
-- Name: AuditLogs FK_AuditLogs_Users_UserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."AuditLogs"
    ADD CONSTRAINT "FK_AuditLogs_Users_UserID" FOREIGN KEY ("UserID") REFERENCES public."Users"("UserID") ON DELETE CASCADE;


--
-- Name: Beds FK_Beds_Rooms_RoomID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Beds"
    ADD CONSTRAINT "FK_Beds_Rooms_RoomID" FOREIGN KEY ("RoomID") REFERENCES public."Rooms"("RoomID") ON DELETE CASCADE;


--
-- Name: ChartAccounts FK_ChartAccounts_ChartAccounts_ParentAccountID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ChartAccounts"
    ADD CONSTRAINT "FK_ChartAccounts_ChartAccounts_ParentAccountID" FOREIGN KEY ("ParentAccountID") REFERENCES public."ChartAccounts"("AccountID") ON DELETE RESTRICT;


--
-- Name: CultureSensitivities FK_CultureSensitivities_LabOrderItems_LabOrderItemID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CultureSensitivities"
    ADD CONSTRAINT "FK_CultureSensitivities_LabOrderItems_LabOrderItemID" FOREIGN KEY ("LabOrderItemID") REFERENCES public."LabOrderItems"("LabOrderItemID") ON DELETE CASCADE;


--
-- Name: CustomAssessmentTemplates FK_CustomAssessmentTemplates_DoctorProfiles_DoctorID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CustomAssessmentTemplates"
    ADD CONSTRAINT "FK_CustomAssessmentTemplates_DoctorProfiles_DoctorID" FOREIGN KEY ("DoctorID") REFERENCES public."DoctorProfiles"("DoctorID") ON DELETE SET NULL;


--
-- Name: DispenseRecords FK_DispenseRecords_Medications_MedicationID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DispenseRecords"
    ADD CONSTRAINT "FK_DispenseRecords_Medications_MedicationID" FOREIGN KEY ("MedicationID") REFERENCES public."Medications"("MedicationID");


--
-- Name: DispenseRecords FK_DispenseRecords_Prescriptions_PrescriptionID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DispenseRecords"
    ADD CONSTRAINT "FK_DispenseRecords_Prescriptions_PrescriptionID" FOREIGN KEY ("PrescriptionID") REFERENCES public."Prescriptions"("PrescriptionID") ON DELETE CASCADE;


--
-- Name: DispenseRecords FK_DispenseRecords_Users_DispensedByUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DispenseRecords"
    ADD CONSTRAINT "FK_DispenseRecords_Users_DispensedByUserID" FOREIGN KEY ("DispensedByUserID") REFERENCES public."Users"("UserID") ON DELETE CASCADE;


--
-- Name: DoctorCommissions FK_DoctorCommissions_Users_DoctorID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DoctorCommissions"
    ADD CONSTRAINT "FK_DoctorCommissions_Users_DoctorID" FOREIGN KEY ("DoctorID") REFERENCES public."Users"("UserID") ON DELETE CASCADE;


--
-- Name: DoctorProfiles FK_DoctorProfiles_Users_UserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."DoctorProfiles"
    ADD CONSTRAINT "FK_DoctorProfiles_Users_UserID" FOREIGN KEY ("UserID") REFERENCES public."Users"("UserID") ON DELETE CASCADE;


--
-- Name: EmployeeCourses FK_EmployeeCourses_EmployeeProfiles_EmployeeID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EmployeeCourses"
    ADD CONSTRAINT "FK_EmployeeCourses_EmployeeProfiles_EmployeeID" FOREIGN KEY ("EmployeeID") REFERENCES public."EmployeeProfiles"("EmployeeID") ON DELETE CASCADE;


--
-- Name: EmployeeLeaves FK_EmployeeLeaves_EmployeeProfiles_EmployeeID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EmployeeLeaves"
    ADD CONSTRAINT "FK_EmployeeLeaves_EmployeeProfiles_EmployeeID" FOREIGN KEY ("EmployeeID") REFERENCES public."EmployeeProfiles"("EmployeeID") ON DELETE CASCADE;


--
-- Name: EmployeeLeaves FK_EmployeeLeaves_Users_ApprovedByUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EmployeeLeaves"
    ADD CONSTRAINT "FK_EmployeeLeaves_Users_ApprovedByUserID" FOREIGN KEY ("ApprovedByUserID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: EmployeeProfiles FK_EmployeeProfiles_Users_UserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."EmployeeProfiles"
    ADD CONSTRAINT "FK_EmployeeProfiles_Users_UserID" FOREIGN KEY ("UserID") REFERENCES public."Users"("UserID") ON DELETE SET NULL;


--
-- Name: InpatientCareExecutions FK_InpatientCareExecutions_InpatientCareOrders_OrderID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InpatientCareExecutions"
    ADD CONSTRAINT "FK_InpatientCareExecutions_InpatientCareOrders_OrderID" FOREIGN KEY ("OrderID") REFERENCES public."InpatientCareOrders"("OrderID") ON DELETE CASCADE;


--
-- Name: InpatientCareExecutions FK_InpatientCareExecutions_Users_ExecutedByUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InpatientCareExecutions"
    ADD CONSTRAINT "FK_InpatientCareExecutions_Users_ExecutedByUserID" FOREIGN KEY ("ExecutedByUserID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: InpatientCareOrders FK_InpatientCareOrders_Admissions_AdmissionID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InpatientCareOrders"
    ADD CONSTRAINT "FK_InpatientCareOrders_Admissions_AdmissionID" FOREIGN KEY ("AdmissionID") REFERENCES public."Admissions"("AdmissionID") ON DELETE CASCADE;


--
-- Name: InpatientCareOrders FK_InpatientCareOrders_HealthServices_HealthServiceID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InpatientCareOrders"
    ADD CONSTRAINT "FK_InpatientCareOrders_HealthServices_HealthServiceID" FOREIGN KEY ("HealthServiceID") REFERENCES public."HealthServices"("ServiceID");


--
-- Name: InpatientCareOrders FK_InpatientCareOrders_Users_CreatedByUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InpatientCareOrders"
    ADD CONSTRAINT "FK_InpatientCareOrders_Users_CreatedByUserID" FOREIGN KEY ("CreatedByUserID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: InpatientDailyLogs FK_InpatientDailyLogs_Admissions_AdmissionID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InpatientDailyLogs"
    ADD CONSTRAINT "FK_InpatientDailyLogs_Admissions_AdmissionID" FOREIGN KEY ("AdmissionID") REFERENCES public."Admissions"("AdmissionID") ON DELETE CASCADE;


--
-- Name: InpatientDailyLogs FK_InpatientDailyLogs_Users_LoggedByUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InpatientDailyLogs"
    ADD CONSTRAINT "FK_InpatientDailyLogs_Users_LoggedByUserID" FOREIGN KEY ("LoggedByUserID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: InventoryCategories FK_InventoryCategories_InventoryCategories_ParentCategoryID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InventoryCategories"
    ADD CONSTRAINT "FK_InventoryCategories_InventoryCategories_ParentCategoryID" FOREIGN KEY ("ParentCategoryID") REFERENCES public."InventoryCategories"("CategoryID") ON DELETE RESTRICT;


--
-- Name: InventoryItems FK_InventoryItems_InventoryCategories_CategoryID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InventoryItems"
    ADD CONSTRAINT "FK_InventoryItems_InventoryCategories_CategoryID" FOREIGN KEY ("CategoryID") REFERENCES public."InventoryCategories"("CategoryID") ON DELETE RESTRICT;


--
-- Name: InventoryItems FK_InventoryItems_Medications_MedicationID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."InventoryItems"
    ADD CONSTRAINT "FK_InventoryItems_Medications_MedicationID" FOREIGN KEY ("MedicationID") REFERENCES public."Medications"("MedicationID") ON DELETE RESTRICT;


--
-- Name: Invoices FK_Invoices_Appointments_AppointmentID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Invoices"
    ADD CONSTRAINT "FK_Invoices_Appointments_AppointmentID" FOREIGN KEY ("AppointmentID") REFERENCES public."Appointments"("AppID") ON DELETE RESTRICT;


--
-- Name: Invoices FK_Invoices_DispenseRecords_DispenseRecordID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Invoices"
    ADD CONSTRAINT "FK_Invoices_DispenseRecords_DispenseRecordID" FOREIGN KEY ("DispenseRecordID") REFERENCES public."DispenseRecords"("DispenseID") ON DELETE RESTRICT;


--
-- Name: Invoices FK_Invoices_DoctorCommissions_DoctorCommissionID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Invoices"
    ADD CONSTRAINT "FK_Invoices_DoctorCommissions_DoctorCommissionID" FOREIGN KEY ("DoctorCommissionID") REFERENCES public."DoctorCommissions"("CommissionID");


--
-- Name: Invoices FK_Invoices_LabOrders_LabOrderID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Invoices"
    ADD CONSTRAINT "FK_Invoices_LabOrders_LabOrderID" FOREIGN KEY ("LabOrderID") REFERENCES public."LabOrders"("LabOrderID");


--
-- Name: Invoices FK_Invoices_RadiologyOrders_RadiologyOrderID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Invoices"
    ADD CONSTRAINT "FK_Invoices_RadiologyOrders_RadiologyOrderID" FOREIGN KEY ("RadiologyOrderID") REFERENCES public."RadiologyOrders"("RadiologyOrderID");


--
-- Name: Invoices FK_Invoices_Users_DoctorID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Invoices"
    ADD CONSTRAINT "FK_Invoices_Users_DoctorID" FOREIGN KEY ("DoctorID") REFERENCES public."Users"("UserID");


--
-- Name: Invoices FK_Invoices_Users_PatientUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Invoices"
    ADD CONSTRAINT "FK_Invoices_Users_PatientUserID" FOREIGN KEY ("PatientUserID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: JournalEntries FK_JournalEntries_Users_CreatedByUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."JournalEntries"
    ADD CONSTRAINT "FK_JournalEntries_Users_CreatedByUserID" FOREIGN KEY ("CreatedByUserID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: JournalEntries FK_JournalEntries_Users_PostedByUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."JournalEntries"
    ADD CONSTRAINT "FK_JournalEntries_Users_PostedByUserID" FOREIGN KEY ("PostedByUserID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: JournalEntryLines FK_JournalEntryLines_ChartAccounts_AccountID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."JournalEntryLines"
    ADD CONSTRAINT "FK_JournalEntryLines_ChartAccounts_AccountID" FOREIGN KEY ("AccountID") REFERENCES public."ChartAccounts"("AccountID") ON DELETE RESTRICT;


--
-- Name: JournalEntryLines FK_JournalEntryLines_JournalEntries_JournalEntryID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."JournalEntryLines"
    ADD CONSTRAINT "FK_JournalEntryLines_JournalEntries_JournalEntryID" FOREIGN KEY ("JournalEntryID") REFERENCES public."JournalEntries"("JournalEntryID") ON DELETE CASCADE;


--
-- Name: LabOrderItems FK_LabOrderItems_LabOrders_LabOrderID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."LabOrderItems"
    ADD CONSTRAINT "FK_LabOrderItems_LabOrders_LabOrderID" FOREIGN KEY ("LabOrderID") REFERENCES public."LabOrders"("LabOrderID") ON DELETE CASCADE;


--
-- Name: LabOrderItems FK_LabOrderItems_LabTests_LabTestID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."LabOrderItems"
    ADD CONSTRAINT "FK_LabOrderItems_LabTests_LabTestID" FOREIGN KEY ("LabTestID") REFERENCES public."LabTests"("LabTestID") ON DELETE RESTRICT;


--
-- Name: LabOrders FK_LabOrders_LabTests_LabTestID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."LabOrders"
    ADD CONSTRAINT "FK_LabOrders_LabTests_LabTestID" FOREIGN KEY ("LabTestID") REFERENCES public."LabTests"("LabTestID") ON DELETE CASCADE;


--
-- Name: LabOrders FK_LabOrders_Users_DoctorID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."LabOrders"
    ADD CONSTRAINT "FK_LabOrders_Users_DoctorID" FOREIGN KEY ("DoctorID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: LabOrders FK_LabOrders_Users_PatientUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."LabOrders"
    ADD CONSTRAINT "FK_LabOrders_Users_PatientUserID" FOREIGN KEY ("PatientUserID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: LabReferenceRanges FK_LabReferenceRanges_LabTests_LabTestID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."LabReferenceRanges"
    ADD CONSTRAINT "FK_LabReferenceRanges_LabTests_LabTestID" FOREIGN KEY ("LabTestID") REFERENCES public."LabTests"("LabTestID") ON DELETE CASCADE;


--
-- Name: LabTests FK_LabTests_LabDevices_DeviceID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."LabTests"
    ADD CONSTRAINT "FK_LabTests_LabDevices_DeviceID" FOREIGN KEY ("DeviceID") REFERENCES public."LabDevices"("LabDeviceID");


--
-- Name: LabTests FK_LabTests_LabTests_PanelID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."LabTests"
    ADD CONSTRAINT "FK_LabTests_LabTests_PanelID" FOREIGN KEY ("PanelID") REFERENCES public."LabTests"("LabTestID") ON DELETE RESTRICT;


--
-- Name: MedicalRecords FK_MedicalRecords_Appointments_AppID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."MedicalRecords"
    ADD CONSTRAINT "FK_MedicalRecords_Appointments_AppID" FOREIGN KEY ("AppID") REFERENCES public."Appointments"("AppID") ON DELETE CASCADE;


--
-- Name: MedicationRequests FK_MedicationRequests_Users_DoctorUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."MedicationRequests"
    ADD CONSTRAINT "FK_MedicationRequests_Users_DoctorUserID" FOREIGN KEY ("DoctorUserID") REFERENCES public."Users"("UserID") ON DELETE CASCADE;


--
-- Name: PatientAssessments FK_PatientAssessments_CustomAssessmentTemplates_TemplateID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PatientAssessments"
    ADD CONSTRAINT "FK_PatientAssessments_CustomAssessmentTemplates_TemplateID" FOREIGN KEY ("TemplateID") REFERENCES public."CustomAssessmentTemplates"("TemplateID") ON DELETE RESTRICT;


--
-- Name: PatientAssessments FK_PatientAssessments_Users_PatientUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PatientAssessments"
    ADD CONSTRAINT "FK_PatientAssessments_Users_PatientUserID" FOREIGN KEY ("PatientUserID") REFERENCES public."Users"("UserID") ON DELETE CASCADE;


--
-- Name: PatientProfiles FK_PatientProfiles_Users_UserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PatientProfiles"
    ADD CONSTRAINT "FK_PatientProfiles_Users_UserID" FOREIGN KEY ("UserID") REFERENCES public."Users"("UserID") ON DELETE CASCADE;


--
-- Name: Prescriptions FK_Prescriptions_MedicalRecords_RecordID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Prescriptions"
    ADD CONSTRAINT "FK_Prescriptions_MedicalRecords_RecordID" FOREIGN KEY ("RecordID") REFERENCES public."MedicalRecords"("RecordID") ON DELETE CASCADE;


--
-- Name: Prescriptions FK_Prescriptions_Medications_MedicationID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Prescriptions"
    ADD CONSTRAINT "FK_Prescriptions_Medications_MedicationID" FOREIGN KEY ("MedicationID") REFERENCES public."Medications"("MedicationID");


--
-- Name: PsychiatricRecords FK_PsychiatricRecords_MedicalRecords_RecordID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."PsychiatricRecords"
    ADD CONSTRAINT "FK_PsychiatricRecords_MedicalRecords_RecordID" FOREIGN KEY ("RecordID") REFERENCES public."MedicalRecords"("RecordID") ON DELETE CASCADE;


--
-- Name: RadiologyOrders FK_RadiologyOrders_Users_DoctorID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."RadiologyOrders"
    ADD CONSTRAINT "FK_RadiologyOrders_Users_DoctorID" FOREIGN KEY ("DoctorID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: RadiologyOrders FK_RadiologyOrders_Users_PatientUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."RadiologyOrders"
    ADD CONSTRAINT "FK_RadiologyOrders_Users_PatientUserID" FOREIGN KEY ("PatientUserID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: RadiologyOrders FK_RadiologyOrders_Users_RadiologistID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."RadiologyOrders"
    ADD CONSTRAINT "FK_RadiologyOrders_Users_RadiologistID" FOREIGN KEY ("RadiologistID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: Rooms FK_Rooms_Wards_WardID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Rooms"
    ADD CONSTRAINT "FK_Rooms_Wards_WardID" FOREIGN KEY ("WardID") REFERENCES public."Wards"("WardID") ON DELETE CASCADE;


--
-- Name: SalaryRecords FK_SalaryRecords_EmployeeProfiles_EmployeeID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SalaryRecords"
    ADD CONSTRAINT "FK_SalaryRecords_EmployeeProfiles_EmployeeID" FOREIGN KEY ("EmployeeID") REFERENCES public."EmployeeProfiles"("EmployeeID") ON DELETE CASCADE;


--
-- Name: SalaryRecords FK_SalaryRecords_JournalEntries_JournalEntryID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SalaryRecords"
    ADD CONSTRAINT "FK_SalaryRecords_JournalEntries_JournalEntryID" FOREIGN KEY ("JournalEntryID") REFERENCES public."JournalEntries"("JournalEntryID") ON DELETE RESTRICT;


--
-- Name: SalaryRecords FK_SalaryRecords_Users_CreatedByUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SalaryRecords"
    ADD CONSTRAINT "FK_SalaryRecords_Users_CreatedByUserID" FOREIGN KEY ("CreatedByUserID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: SensitivityResults FK_SensitivityResults_CultureSensitivities_CultureSensitivityID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SensitivityResults"
    ADD CONSTRAINT "FK_SensitivityResults_CultureSensitivities_CultureSensitivityID" FOREIGN KEY ("CultureSensitivityID") REFERENCES public."CultureSensitivities"("CultureSensitivityID") ON DELETE CASCADE;


--
-- Name: SoapNotes FK_SoapNotes_MedicalRecords_RecordID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."SoapNotes"
    ADD CONSTRAINT "FK_SoapNotes_MedicalRecords_RecordID" FOREIGN KEY ("RecordID") REFERENCES public."MedicalRecords"("RecordID") ON DELETE CASCADE;


--
-- Name: StockCountItems FK_StockCountItems_InventoryItems_ItemID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockCountItems"
    ADD CONSTRAINT "FK_StockCountItems_InventoryItems_ItemID" FOREIGN KEY ("ItemID") REFERENCES public."InventoryItems"("ItemID") ON DELETE RESTRICT;


--
-- Name: StockCountItems FK_StockCountItems_StockCounts_StockCountID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockCountItems"
    ADD CONSTRAINT "FK_StockCountItems_StockCounts_StockCountID" FOREIGN KEY ("StockCountID") REFERENCES public."StockCounts"("StockCountID") ON DELETE CASCADE;


--
-- Name: StockCounts FK_StockCounts_Users_CreatedByUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockCounts"
    ADD CONSTRAINT "FK_StockCounts_Users_CreatedByUserID" FOREIGN KEY ("CreatedByUserID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: StockCounts FK_StockCounts_Users_PostedByUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockCounts"
    ADD CONSTRAINT "FK_StockCounts_Users_PostedByUserID" FOREIGN KEY ("PostedByUserID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: StockCounts FK_StockCounts_Users_ReversedByUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockCounts"
    ADD CONSTRAINT "FK_StockCounts_Users_ReversedByUserID" FOREIGN KEY ("ReversedByUserID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: StockCounts FK_StockCounts_Warehouses_WarehouseID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockCounts"
    ADD CONSTRAINT "FK_StockCounts_Warehouses_WarehouseID" FOREIGN KEY ("WarehouseID") REFERENCES public."Warehouses"("WarehouseID") ON DELETE RESTRICT;


--
-- Name: StockMovementItems FK_StockMovementItems_InventoryItems_ItemID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockMovementItems"
    ADD CONSTRAINT "FK_StockMovementItems_InventoryItems_ItemID" FOREIGN KEY ("ItemID") REFERENCES public."InventoryItems"("ItemID") ON DELETE RESTRICT;


--
-- Name: StockMovementItems FK_StockMovementItems_StockMovements_MovementID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockMovementItems"
    ADD CONSTRAINT "FK_StockMovementItems_StockMovements_MovementID" FOREIGN KEY ("MovementID") REFERENCES public."StockMovements"("MovementID") ON DELETE CASCADE;


--
-- Name: StockMovements FK_StockMovements_Users_CreatedByUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockMovements"
    ADD CONSTRAINT "FK_StockMovements_Users_CreatedByUserID" FOREIGN KEY ("CreatedByUserID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: StockMovements FK_StockMovements_Users_PostedByUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockMovements"
    ADD CONSTRAINT "FK_StockMovements_Users_PostedByUserID" FOREIGN KEY ("PostedByUserID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: StockMovements FK_StockMovements_Warehouses_ToWarehouseID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockMovements"
    ADD CONSTRAINT "FK_StockMovements_Warehouses_ToWarehouseID" FOREIGN KEY ("ToWarehouseID") REFERENCES public."Warehouses"("WarehouseID") ON DELETE RESTRICT;


--
-- Name: StockMovements FK_StockMovements_Warehouses_WarehouseID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."StockMovements"
    ADD CONSTRAINT "FK_StockMovements_Warehouses_WarehouseID" FOREIGN KEY ("WarehouseID") REFERENCES public."Warehouses"("WarehouseID") ON DELETE RESTRICT;


--
-- Name: TelemedicineSessions FK_TelemedicineSessions_Appointments_AppointmentID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."TelemedicineSessions"
    ADD CONSTRAINT "FK_TelemedicineSessions_Appointments_AppointmentID" FOREIGN KEY ("AppointmentID") REFERENCES public."Appointments"("AppID") ON DELETE CASCADE;


--
-- Name: Treasuries FK_Treasuries_ChartAccounts_AccountID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Treasuries"
    ADD CONSTRAINT "FK_Treasuries_ChartAccounts_AccountID" FOREIGN KEY ("AccountID") REFERENCES public."ChartAccounts"("AccountID") ON DELETE RESTRICT;


--
-- Name: UserNotifications FK_UserNotifications_Users_UserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."UserNotifications"
    ADD CONSTRAINT "FK_UserNotifications_Users_UserID" FOREIGN KEY ("UserID") REFERENCES public."Users"("UserID") ON DELETE CASCADE;


--
-- Name: Users FK_Users_Treasuries_AssignedTreasuryID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "FK_Users_Treasuries_AssignedTreasuryID" FOREIGN KEY ("AssignedTreasuryID") REFERENCES public."Treasuries"("TreasuryID") ON DELETE RESTRICT;


--
-- Name: Vouchers FK_Vouchers_Appointments_AppointmentID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Vouchers"
    ADD CONSTRAINT "FK_Vouchers_Appointments_AppointmentID" FOREIGN KEY ("AppointmentID") REFERENCES public."Appointments"("AppID") ON DELETE RESTRICT;


--
-- Name: Vouchers FK_Vouchers_ChartAccounts_AccountID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Vouchers"
    ADD CONSTRAINT "FK_Vouchers_ChartAccounts_AccountID" FOREIGN KEY ("AccountID") REFERENCES public."ChartAccounts"("AccountID") ON DELETE RESTRICT;


--
-- Name: Vouchers FK_Vouchers_Invoices_InvoiceID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Vouchers"
    ADD CONSTRAINT "FK_Vouchers_Invoices_InvoiceID" FOREIGN KEY ("InvoiceID") REFERENCES public."Invoices"("InvoiceID") ON DELETE RESTRICT;


--
-- Name: Vouchers FK_Vouchers_Treasuries_ToTreasuryID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Vouchers"
    ADD CONSTRAINT "FK_Vouchers_Treasuries_ToTreasuryID" FOREIGN KEY ("ToTreasuryID") REFERENCES public."Treasuries"("TreasuryID") ON DELETE RESTRICT;


--
-- Name: Vouchers FK_Vouchers_Treasuries_TreasuryID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Vouchers"
    ADD CONSTRAINT "FK_Vouchers_Treasuries_TreasuryID" FOREIGN KEY ("TreasuryID") REFERENCES public."Treasuries"("TreasuryID") ON DELETE RESTRICT;


--
-- Name: Vouchers FK_Vouchers_Users_CreatedByUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Vouchers"
    ADD CONSTRAINT "FK_Vouchers_Users_CreatedByUserID" FOREIGN KEY ("CreatedByUserID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: Vouchers FK_Vouchers_Users_PatientUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Vouchers"
    ADD CONSTRAINT "FK_Vouchers_Users_PatientUserID" FOREIGN KEY ("PatientUserID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: Vouchers FK_Vouchers_Users_PostedByUserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Vouchers"
    ADD CONSTRAINT "FK_Vouchers_Users_PostedByUserID" FOREIGN KEY ("PostedByUserID") REFERENCES public."Users"("UserID") ON DELETE RESTRICT;


--
-- Name: WebPushSubscriptions FK_WebPushSubscriptions_Users_UserID; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."WebPushSubscriptions"
    ADD CONSTRAINT "FK_WebPushSubscriptions_Users_UserID" FOREIGN KEY ("UserID") REFERENCES public."Users"("UserID") ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--


