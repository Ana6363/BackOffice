CREATE TABLE Appointements (
    AppointementId CHAR(36) NOT NULL, -- Primary key as a UUID (GUID)
    Schedule DATETIME NOT NULL,       -- Scheduled date and time
    Request VARCHAR(255) NOT NULL,    -- Foreign key reference to OperationRequest (assuming OperationRequestId is a string)
    Patient VARCHAR(255) NOT NULL,    -- Foreign key reference to Patient (assuming PatientId is a string)
    Staff VARCHAR(255) NOT NULL,      -- Foreign key reference to Staff (assuming StaffId is a string),
    
    PRIMARY KEY (AppointementId),

    -- Assuming foreign key constraints to respective tables
    CONSTRAINT FK_Appointement_OperationRequest FOREIGN KEY (Request) REFERENCES OperationRequests(RequestId) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_Appointement_Patient FOREIGN KEY (Patient) REFERENCES Patients(RecordNumber) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_Appointement_Staff FOREIGN KEY (Staff) REFERENCES Staff(StaffId) ON DELETE CASCADE ON UPDATE CASCADE
);
