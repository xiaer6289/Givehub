# 🎯 GiveHub - Donation Management System

## 📋 Overview
GiveHub is a comprehensive donation management system designed to ensure organizational transparency in handling charitable donations. The platform facilitates seamless connections between donors and donees (beneficiaries) while providing robust administrative controls.

---

## ✨ Key Features

### 🔐 Security & Authentication
- **Secure Login System**: All users must authenticate before performing any donation activities
- **Password Encryption**: User passwords are encrypted using industry-standard hashing algorithms
- **Complex Password Requirements**: 
  - Minimum 8 characters
  - Must include uppercase letters (A-Z)
  - Must include lowercase letters (a-z)
  - Must include numbers (0-9)
  - Must include special symbols (!@#$%^&*)
- **Two-Factor Authentication (Admin)**: Admin login requires email verification after credential validation

---

## 👥 User Roles

### 💝 Donor Features

#### Payment Integration
- **Stripe Integration**: Secure monetary donations processed through Stripe payment gateway
- **Real-time Processing**: Instant payment confirmation and receipt generation

#### Donation Options
- **Browse Donees**: View detailed profiles of different beneficiaries
- **Monetary Donations**: Contribute funds directly via Stripe
- **Item Donations**: 
  - View items specifically requested by donees
  - Select items to donate from donee's requirement list
  - Schedule delivery date for item donations
  - Specify destination for item delivery

#### Donation Management
- **Donation History**: Track all past donations (monetary and items)
- **Status Tracking**: Monitor approval status of item donations

---

### 🏥 Donee (Beneficiary) Features

#### Registration & Communication
- **📧 Contact Form Submission**: 
  - Donees submit applications via SMTP-powered contact form
  - Forms sent directly to admin email for review
  - Include needs assessment and organization details

#### Profile Management
- **📝 Requirement List**: Specify needed items for donation
- **📍 Location Details**: Provide delivery destination information
- **📊 Profile Visibility**: Viewable by all registered donors

---

### 👨‍💼 Admin Features

#### 🔒 Enhanced Security
- **Email Verification**: Two-step login process with email confirmation code
- **Session Management**: Secure admin session handling

#### 📦 Donation Management
- **📬 Notification System**: Receive alerts when donees confirm item receipt
- **✅ Approval Workflow**: 
  - Review item donation requests
  - Approve or reject donations based on verification
  - Track donation fulfillment status

#### 👥 Donor Management
- **View Donor Profiles**: Access comprehensive donor information
- **📜 Donation History**: View detailed records of each donor's contributions
- **Item Tracking**: Monitor all item donations per donor

#### 🏢 Donee Management (CRUD Operations)
- **➕ Create**: Add new donee profiles
- **📖 Read**: View donee details and requirements
- **✏️ Update**: Modify donee information and needs
- **🗑️ Delete**: Remove donee profiles when necessary

#### 🔍 Advanced Features
- **📄 Pagination**: Navigate large datasets efficiently
- **🔎 Search Functionality**: Quick lookup of donors/donees by name, date, or criteria
- **🎯 Filtering Options**: 
  - Filter by donation type (money/items)
  - Filter by date range
  - Filter by status (pending/approved/rejected)
  - Filter by amount range

---

## 🛠️ Technical Stack

### Payment Processing
- **Stripe API**: Secure payment gateway integration
- **Session Management**: Checkout session handling with success/cancel callbacks

### Communication
- **SMTP Integration**: Email delivery for contact forms and notifications
- **Email Verification**: Two-factor authentication for admin accounts

### Security
- **Password Hashing**: Encrypted password storage
- **Role-Based Access Control**: Separate permissions for donors, donees, and admins
- **Input Validation**: Complex password requirements enforcement

---

## 🚀 Getting Started

### Prerequisites
- .NET Core SDK
- SQL Server
- Stripe Account (API Keys)
- SMTP Server Credentials

### Installation
1. Clone the repository
2. Configure `appsettings.json` with:
   - Database connection string
   - Stripe API keys (SecretKey, PublishableKey)
   - SMTP credentials
3. Run database migrations
4. Launch the application

---

## 📊 System Workflow

```
1. Donor Registration → Login → Browse Donees
                                    ↓
2. Select Donee → Choose Donation Type (Money/Items)
                                    ↓
3. Money: Stripe Payment → Instant Confirmation
   Items: Select Items → Choose Date → Submit Request
                                    ↓
4. Admin Receives Notification → Review Request
                                    ↓
5. Admin Approves/Rejects → Donor Notified
                                    ↓
6. Item Delivered → Donee Confirms → Admin Updates Status
```

---

## 🎯 Transparency Features
- Complete audit trail of all donations
- Real-time donation tracking
- Public donee profiles with specific needs
- Verified approval process
- Donation history accessible to donors
