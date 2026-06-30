namespace WrkPlan.CustomerPortal.Domain.Enums;

public enum SubscriptionCycle
{
    Monthly = 1,
    Quarterly = 2,
    Annual = 3
}

public enum RecordStatus
{
    Active = 1,
    Inactive = 2,
    Draft = 3,
    Overdue = 4
}

public enum OnboardingStepType
{
    InitialAccountSetup = 1,
    Questionnaire = 2,
    RequiredDocs = 3,
    PaymentCheckpoint = 4,
    GoLiveReview = 5
}

