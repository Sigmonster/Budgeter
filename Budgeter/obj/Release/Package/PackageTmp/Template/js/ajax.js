//Manage Section
function GetTransactionsView() {
    $(".manage-head").hide();
    $("#accountManageHeadWrapper").empty();
    $("#createFormPartialWrapper").empty();
    $("#dataListPartialWrapper").empty();
    $("#transactions-head").show();
}
function FetchCategoryViews() {
    $(".manage-head").hide();
    GetCategoryViews();
    $("#categories-head").show();
}
function FetchBudgetViews() {
    $(".manage-head").hide();
    GetBudgetViews();
    $("#budgets-head").show();
}
function FetchAccountsView() {
    $(".manage-head").hide();
    GetAccountsView();
    $("#accounts-head").show();
}

//########################################
//####### Start Account Manage AJAX ######
//########################################
//Get Account Data
function submitGetAccountForm() {
    console.log("submitGetAccountForm() Clicked")
    var id = $("#Accounts").val();
    var form = $("#AccountManageForm");
    $("#createFormPartialWrapper").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
    $("#dataListPartialWrapper").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
    $("#accountManageHeadWrapper").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
    getAccountManageHeadPartial(form);
    submitGetAddTransactionPartial();
    activeTransactions(id);
    
}
//Get Initial Account Header
function getAccountManageHeadPartial(form) {
    $.ajax({
        type: "Get",
        url: "/Saver/_AccountManageHeadPartial",
        data: form.serialize(),
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
            console.log("Error _AccountManageHeadPartial " + response);
        },
        success: function (response) {
            console.log("Success! _AccountManageHeadPartial");
            $("#ajax-Accounts-loader1").hide();
            $("#accountManageHeadWrapper").html(response);

        }
    });
};
//Get Account Balances(after initial load)
function updateAccountBalances() {
    $("#display-account-balances").load("/Saver/_AccountBalancesPartial", { id: $("#current-acc-num").val() });
    var accBal = $("#AccBalance").val();
    var recBal = $("#RecBalance").val();
    if (accBal < 0) {
        AccOverdraftNotification("Overdraft Notification", "Your Account Balance is in the Negative.")
    }
    if (recBal < 0) {
        AccOverdraftNotification("Overdraft Notification", "Your Reconciled Balance is in the Negative.")
    }
}
// Get TransactionAddPartial
function submitGetAddTransactionPartial() {
    var form = $("#AccountManageForm");
    $.ajax({
        beforeSend: function () {
            $("#createFormPartialWrapper").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
            $("#dataListPartialWrapper").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
        },
        type: "Get",
        url: "/Saver/_AddTransactionPartial",
        data: form.serialize(),
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
            console.log("Error _AddTransactionPartial! " + response);
        },
        success: function (response) {
            console.log("Success! _AddTransactionPartial");
            $("#createFormPartialWrapper").html(response);

        }
    });
};
//Get Active Transactions : _DisplayTransactionsPartial
function activeTransactions(accountId) {
    var id = parseInt(accountId);
    $.ajax({
        type: "Get",
        url: "/Saver/_DisplayTransactionsPartial",
        data: { accounts: id },
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
            console.log("Error _DisplayTransactionsPartial! " + response);
        },
        success: function (response) {
            console.log("Success! _DisplayTransactionsPartial");
            updateAccountBalances();            
            $("#dataListPartialWrapper").html(response);
        }
    });

};

//Get Archived Transactions :  _DisplayTransactionsPartial
function archivedTransactions(id) {
    console.log("archicedTransactions Clicked, ID:" + id)
    $.ajax({
        type: "Get",
        url: "/Saver/DisplayArchivedTransactions",
        data: { id: id },
        dataType: 'text',
        error: function (archivedrRsponse) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
            console.log("Error archivedTransactions(id)! " + archivedrRsponse);
        },
        success: function (archivedrRsponse) {
            console.log("Success! archivedTransactions(id)");
            $("#dataListPartialWrapper").empty();
            $("#dataListPartialWrapper").html(archivedrRsponse);
        }
    })
};

//Get Transaction To Edit(in Modal) : _EditTransactionPartial
function editTransactionBtn(id) {
    console.log("Edit Clicked, ID:" + id);
    var dropdownElement = "#transactionDropdown-" + id;
    $("#editModal-title").html('<div class="text-center"><h4>Edit Transaction</h4></div>');
    $(dropdownElement).click();

    $.ajax({
        beforeSend: function () {
            $("#editModal-Body").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
            $("#editModal").modal('show');
        },
        type: "Get",
        url: "/Saver/EditTransaction",
        data: { id: id },
        dataType: 'text',
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
            console.log("Error editTransactionBtn(id)! " + response);
        },
        success: function (response) {
            console.log("Success! _EditTransactionPartial");
            $("#editModal-Body").html(response);
        }
    })
};
//Get Quick Add Form/Modal
function QuickAddTransaction() {

    $.ajax({
        beforeSend: function () {
            $("#quickAddModal-Body").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
            $("#quickAddModal").modal('show');
        },
        type: "Get",
        url: "/Saver/QuickAddTransaction",
        dataType: 'text',
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
            console.log("Error editTransactionBtn(id)! " + response);
        },
        success: function (response) {
            console.log("Success! _QuickAddTransactionPartial");
            if (response == "Error") {
                ErrorNotification("Error", "You are not in a Household, or your Household has no Accounts. You need to join a Household, or add Accounts to before adding Transactions.");
                $("#quickAddModal").modal('hide');
            }
            else {
                $("#quickAddModal-Body").html(response);
            }
        }
    })
};
// GET : Transaction Header : _TransactionsManagerDropdownPartial
function UpdateTransactionManagerHead(){
    $.ajax({
        beforeSend: function () {
            $("#transactions-head").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
        },
        type: "Get",
        url: "/Saver/UpdateTransactionManagerHead",
        dataType: 'text',
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
            console.log("Error UpdateTransactionManagerHead()! " + response);
        },
        success: function (response) {
            console.log("Success! _TransactionsManagerDropdownPartial");
            if (response == "Error") {
                ErrorNotification("Error", "There was an error please refresh your page. If you continue to have this issue contact the support desk.");
            }
            else {
                $("#transactions-head").html(response);
            }
        }
    })
};
//################################
//##### POST Transaction AJAX ####
//################################
//Post Quick Add Transaction
function submitQuickAddTransactionForm() {
    var form = $("#quickAddTransactionFormId");
    $.ajax({
        beforeSend: function () {
            $("#quickAddModal-Body").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
        },
        type: "Post",
        url: "/Saver/QuickAddTransaction",
        data: form.serialize(),
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
            console.log("Error submitQuickAddTransactionForm()! " + response);
        },
        success: function (response) {
            console.log("Success! submitQuickAddTransactionForm()");
            if (response == "Success") {
                $("#quickAddModal").modal('hide');
                SuccessNotification("Save Sucessful!", "Form submission was successful and data was saved!");
            }
            else {
                $("#quickAddModal-Body").html(response);
                ErrorNotification("Error", "There was an error, please try again. If you continue to have problems please contact support.");
            }
        }
    })
};
// Post Edit Transaction : _DisplayTransactionsPartial
function submitEditTransactionForm() {
    var form = $("#editTransactionModalFormId");
    var accountId = $("#current-acc-num").val();
    $.ajax({
        beforeSend: function () {
            $("#editModal-Body").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
        },
        type: "Post",
        url: "/Saver/EditTransaction",
        data: form.serialize(),
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
            console.log("Error submitEditTransactionForm()! " + response);
        },
        success: function (response) {
            console.log("Success! submitEditTransactionForm()");
            if (response == "Success") {
                $("#editModal").modal('hide');
                SuccessNotification("Save Sucessful!", "Form submission was successful and data was saved!");
                activeTransactions(accountId);
                submitGetAddTransactionPartial();
            }
            else {
                $("#editModal-Body").html(response);
                ErrorNotification("Error", "There was an error, please try again. If you continue to have problems please contact support.");
            }
        }
    })
};

//Post Add Transation: _DisplayTransactionPartial, _AddTransactionPartial(Reset)
function submitAddTransactionForm() {
    var form = $("#addTransactionFormId");
    var accountId = $("#current-acc-num").val();

    $.ajax({
        type: "POST",
        url: "/Saver/CreateTransaction",
        data: form.serialize(),
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
            console.log("Error submitAddTransactionForm()! " + response);
        },
        success: function (response) {
            console.log("Success! submitAddTransactionForm()");
              if (response == "Success") {
                $("#editModal").modal('hide');
                SuccessNotification("Save Sucessful!", "Form submission was successful and data was saved!");
                //$("#display-account-balances").load("/Saver/_AccountBalancesPartial", { id: accountId });
                activeTransactions(accountId);
                submitGetAddTransactionPartial()
                //$("#createFormPartialWrapper").load("/Saver/_AddTransactionPartial", { accounts: accountId });
                //var accBal = $("#AccBalance").val();
                //var recBal = $("#RecBalance").val();
                //if (accBal < 0) {
                //    AccOverdraftNotification("Overdraft Notification", "Your ACCOUNT BALANCE is in the Negative.")
                //}
                //if (recBal < 0) {
                //    AccOverdraftNotification("Overdraft Notification", "Your RECONCILED BALANCE is in the Negative.")
                //}
            }
            else {
                  $("#createFormPartialWrapper").html(response);
                ErrorNotification("Error", "There was an error, please try again. If you continue to have problems please contact support.");
            }
        }
    })
};
//TODO:Charts, BudgetItem(items), User Index, My Household or other Dashboard. Fix Quick ADD, Make Head of household Remove Users Option
// Post Void/Unvoid Transactions : _DisplayTransactionsPartial
function voidBtn(id, voidAction) {
    console.log("Clicked, ID:" + id);
    var rowElement = "#transaction-row-" + id;
    var dropDownElement = "#transactionDropdown-" + id;
    $.ajax({
        type: "POST",
        url: "/Saver/VoidTransaction",
        data: { id: id, voidAction: voidAction },
        dataType: 'text',
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
            console.log("Error! " + response);
        },
        success: function (response) {
            console.log("Success!");
            if (response == "True") {
                if($(rowElement).hasClass("voided-False"))
                    $(rowElement).removeClass().addClass("voided-True");
                SuccessNotification("Save Sucessful!", "Form submission was successful and data was saved!");
                updateAccountBalances();
            }
            else if (response == "False") {
                if($(rowElement).hasClass("voided-True"))
                    $(rowElement).removeClass().addClass("voided-False");
                SuccessNotification("Save Sucessful!", "Form submission was successful and data was saved!");
                updateAccountBalances();
            }
            else
                ErrorNotification("Error", "There was an error, please try again. If you continue to have problems please contact support.");
            $(dropDownElement).click();
        }
    })
};

// Post Proccessed/Pending Transactions :  _DisplayTransactionsPartial
function processedBtn(id, reconcileAction) {
    console.log("Clicked, ID:" + id);
    var expenseElement = "#reconciled-" + id;
    var dropDownElement = "#transactionDropdown-" + id;
    $.ajax({
        type: "POST",
        url: "/Saver/ReconcileTransaction",
        data: { id: id, reconcileAction: reconcileAction },
        dataType: 'text',
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
            console.log("Error! " + response);
        },
        success: function (response) {
            console.log("Success!");
            if (response == "True") {
                $(expenseElement).removeClass().addClass("isReconciled-True");
                $(expenseElement).html("Processed");
                updateAccountBalances();
                SuccessNotification("Save Sucessful!", "Form submission was successful and data was saved!");
            }
            else if (response == "False") {
                $(expenseElement).removeClass().addClass("isReconciled-False");
                $(expenseElement).html("Pending");
                updateAccountBalances();
                SuccessNotification("Save Sucessful!", "Form submission was successful and data was saved!");
            }
            else
                ErrorNotification("Error", "There was an error, please try again. If you continue to have problems please contact support.");
            $(dropDownElement).click();
        }
    })
};

// Post Active/InActive Transactive : Success/Error
// UI Reference Delete Transactions
function deleteBtn(id, isActiveAction) {
    console.log("Clicked, ID:" + id);
    var rowElement = "#transaction-row-" + id;
    var dropDownElement = "#transactionDropdown-" + id;

    $.ajax({
        type: "POST",
        url: "/Saver/ActiveStatusToggleTransaction",
        data: { id: id, isActiveAction: isActiveAction },
        dataType: 'text',
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
            console.log("Error! " + response);
        },
        success: function (response) {
            console.log("Success!");
            if (response == "Success") {
                $(dropDownElement).click();
                $(rowElement).remove();
                updateAccountBalances();
                SuccessNotification("Save Sucessful!", "Form submission was successful and data was saved!");
            }
            else {
                ErrorNotification("Error", "There was an error, please try again. If you continue to have problems please contact support.");
            }
            console.log("end");
        }
    })
};

//########################################
//##### END Account Manage AJAX ##########
//########################################

//########################################
//####### Start Budget Manage AJAX #######
//########################################
//Get Budget Views
function GetBudgetViews() {
    GetAddBudgetPartial();
    GetActiveBudgetsPartial();
}


//Get Add Budget Partial
function GetAddBudgetPartial() {
    $("#createFormPartialWrapper").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
    $("#createFormPartialWrapper").load("/Saver/_BudgetAddPartial");
}
//Get Active Budgets Partial : _DisplayBudgetsPartial
function GetActiveBudgetsPartial() {
    $("#dataListPartialWrapper").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
    $("#dataListPartialWrapper").load("/Saver/DisplayActiveBudgets");
};
function GetInactiveBudgetsPartial() {
    $("#dataListPartialWrapper").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
    $("#dataListPartialWrapper").load("/Saver/DisplayInactiveBudgets");
};

//Get Budget To Edit(in Modal) : _EditBudgetPartial
function editBudgetBtn(id) {
    console.log("Edit Clicked, ID:" + id);
    var dropdownElement = "#budgetDropdown-" + id;
    $(dropdownElement).click();
    $.ajax({
        beforeSend: function () {
            $("#editModal-Body").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
            $("#editModal-title").html('<div class="text-center"><h4>Edit Budget</h4></div>');
            $("#editModal").modal('show');
        },
        type: "Get",
        url: "/Saver/EditBudget",
        data: { id: id },
        dataType: 'text',
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
            console.log("Error editTransactionBtn(id)! " + response);
        },
        success: function (response) {
            console.log("Success! _EditBudgetPartial");
            $("#editModal-Body").html(response);
        }
    })
};
//#############################
//##### Budget AJAX Posts #####
//POST: Add Budget Form
function submitAddBudgetForm() {
    console.log("Click! submitAddBudgetForm()");
    var form = $("#addBudgetForm");
    //$("#addBudgetSubmitWrapper").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>')
    $.ajax({
        type: "Post",
        url: "/Saver/AddBudgetPartial",
        data: form.serialize(),
        dataType: 'text',
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
            console.log("Error! submitAddBudgetForm()" + response);
        },
        success: function (response) {
            console.log("Success! submitAddBudgetForm()");
            if (response == "Success") {
                SuccessNotification("Save Sucessful!", "Form submission was successful and data was saved!");
                GetBudgetViews();
            }
            else if (response == "ErrorDuplicate") {
                ErrorNotification("Duplicate Error", "There is already a budget created with this Category. You can only create one budget per category.");
            }
            else {
                ErrorNotification("Error Form Submission", "There was an error when submitting the form, be sure that all fields are being filled out correctly and try again.");
                $("#createFormPartialWrapper").html(response);
            }
        }
    })
    
};

//POST: Edit Budget Submit Form
function editBudgetSumbitForm(id) {
    console.log("Edit Clicked, ID:" + id);
    var form = $("#editBudgetForm");

    $.ajax({
        beforeSend: function () {
            $("#editModal-Body").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
        },
        type: "Post",
        url: "/Saver/EditBudget",
        data: form.serialize(),
        dataType: 'text',
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
            console.log("Error editBudgetSumbitForm(id)! " + response);
        },
        success: function (response) {
            console.log("Success! editBudgetSumbitForm(id)");
            if (response == "Success") {
                $("#editModal").modal('hide');
                SuccessNotification("Save Sucessful!", "Form submission was successful and data was saved!");
                GetActiveBudgetsPartial();
            }
            else {
                $("#editModal-Body").html(response);
                ErrorNotification("Error", "There was an error, please try again. If you continue to have problems please contact support.");
            }
        }
    })
};
// Post Set Budget Active/InActive : _DisplayBudgetsPartial
// UI Reference Delete Transactions
function deleteBudgetBtn(id, isActiveAction) {
    console.log("Clicked, ID:" + id);
    var dropdownElement = "#budgetDropdown-" + id;
    $(dropdownElement).click();

    $.ajax({
        type: "POST",
        url: "/Saver/ActiveStatusToggleBudget",
        data: { id: id, isActiveAction: isActiveAction },
        dataType: 'text',
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
        },
        success: function (response) {
            console.log("Success! deleteBudgetBtn()");
            if (response == "Success") {
                SuccessNotification("Save Sucessful!", "Form submission was successful and data was saved!");
                var rowElement = ".budget-row-" + id;
                $(rowElement).hide();
            }
            else {
                ErrorNotification("Error", "There was an error, please try again. If you continue to have problems please contact support.");
            }
        }
    })
};

//########################################
//######## End Budget Manage AJAX ########
//########################################

//########################################
//###### Start Category Manage AJAX ######
//########################################

function GetCategoryViews() {
    $("#createFormPartialWrapper").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
    $("#dataListPartialWrapper").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
    GetAddCategoryPartial();
    GetHouseholdCategoriesPartial();
}

//Get Add Category
function GetAddCategoryPartial() {
    console.log("GetAddCategoryPartial() Clicked");
    $("#createFormPartialWrapper").load("/Saver/GetAddCategoryPartial");
};
//Get Household Categories
function GetHouseholdCategoriesPartial(){
    console.log("GetHouseholdCategoriesPartial() Clicked");
    $("#dataListPartialWrapper").load("/Saver/GetHouseholdCategoriesPartial");
};



//Get Category To Edit(in Modal) : _EditCategoryPartial
function editCategoryBtn(id) {
    console.log("Edit Clicked, ID:" + id);
    var categoryId = id;
    var dropdownElement = "#categoryDropdown-" + id;
    $("#editModal-title").html('<div class="text-center"><h4>Edit Category</h4></div>');
    $(dropdownElement).click();
    $.ajax({
        beforeSend: function () {
            $("#editModal-Body").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
            $("#editModal").modal('show');
        },
        type: "Get",
        url: "/Saver/EditCategory",
        data: { id: categoryId },
        dataType: 'text',
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
        },
        success: function (response) {
            console.log("Success! _EditTransactionPartial");
            $("#editModal-Body").html(response);
        }
    })
};
//#############################
//##### POST: Categories ######
//POST: Edit Budget Submit Form
function submitEditCategoryForm() {
    var form = $("#editCategoryForm");
    $.ajax({
        beforeSend: function () {
            $("#editModal-Body").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
        },
        type: "Post",
        url: "/Saver/EditCategory",
        data: form.serialize(),
        dataType: 'text',
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
            $("#editModal-Body").html("<div class='text-center text-danger'><h4 class='text-danger'>Error when submitting form. Form: Category Edit</h4></div>");
        },
        success: function (response) {
            console.log("Success! submitEditCategoryForm()");
            if (response == "Success") {
                $("#editModal").modal('hide');
                SuccessNotification("Save Sucessful!", "Form submission was successful and data was saved!");
                GetHouseholdCategoriesPartial();
            }
            else if (response == "Error:DefaultCategory") {
                $("#editModal").modal('hide');
                ErrorNotification("Error: Default Category", "You can only edit User Created Categories.");
            }
            else {
                $("#editModal-Body").html(response);
                ErrorNotification("Error", "There was an error, please try again. If you continue to have problems please contact support.");
            }
        }
    })
};
function submitAddCategoryForm() {
    var form = $("#addCategoryFormId");
    $.ajax({
        beforeSend: function () {
            form.hide();
            form.after('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
        },
        type: "Post",
        url: "/Saver/AddCategory",
        data: form.serialize(),
        dataType: 'text',
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
        },
        success: function (response) {
            console.log("Success! submitEditCategoryForm()");
            if (response == "Success") {
                SuccessNotification("Save Sucessful!", "Form submission was successful and data was saved!");
                GetHouseholdCategoriesPartial();
                GetAddCategoryPartial();
            }
            else {
                $("#createFormPartialWrapper").html(response);
                ErrorNotification("Error", "There was an error, please try again. If you continue to have problems please contact support.");
            }
        }
    })
}

function toggleCategoryStatusBtn(id, isActiveAction) {

    $.ajax({
        type: "POST",
        url: "/Saver/ActiveStatusToggleCategory",
        data: { id: id, isActiveAction: isActiveAction },
        dataType: 'text',
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
        },
        success: function (response) {
            console.log("Success!");
            if (response == "Success") {
                SuccessNotification("Save Sucessful!", "The request was successful and data was saved!");
                GetHouseholdCategoriesPartial();
            }
            else if (response == "Error:DefaultCategory") {
                ErrorNotification("Error: Default Category", "You can only edit User Created Categories.");
            }
            else {
                ErrorNotification("Error", "There was an error, please try again. If you continue to have problems please contact support.");
            }
        }
    })
};



//########################################
//####### End Category Manage AJAX #######
//########################################

//########################################
//##### Start Account Manage AJAX ########
//########################################
function GetAccountsView() {
    GetAddAccountPartial();
    GetAccountsListPartial();
}
//Get Add Account Partial
function GetAddAccountPartial(){
    $("#createFormPartialWrapper").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
    $("#createFormPartialWrapper").load("/Saver/GetAddAccountPartial");
}
//Get Active Accounts List Partial
function GetAccountsListPartial() {
    $("#dataListPartialWrapper").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
    $("#dataListPartialWrapper").load("/Saver/GetAccountsListPartial");
}
//Get Inactive Accounts List Partial
function GetInactiveAccountsListPartial() {
    $("#dataListPartialWrapper").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
    $("#dataListPartialWrapper").load("/Saver/GetInactiveAccountsListPartial");
}
//Get Account To Edit(in Modal) : _EditAccountPartial
function editAccountBtn(id) {
    console.log("Edit Clicked, ID:" + id);
    var accountId = id;
    var dropdownElement = "#accountDropdown-" + id;
    $("#editModal-title").html('<div class="text-center"><h4>Edit Account</h4></div>');
    $(dropdownElement).click();
    $.ajax({
        beforeSend: function () {
            $("#editModal-Body").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
            $("#editModal").modal('show');
        },
        type: "Get",
        url: "/Saver/EditAccount",
        data: { id: accountId },
        dataType: 'text',
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
        },
        success: function (response) {
            console.log("Success! editAccountBtn()");
            $("#editModal-Body").html(response);
        }
    })
};
//#############################
//## Post Account AJAX Start ##

//POST: Add Account Submit Form
function submitAddAccountForm() {
    var form = $("#addAccountForm");
    $.ajax({
        beforeSend: function () {
            form.hide();
            form.after('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
        },
        type: "Post",
        url: "/Saver/SubmitAddAccountForm",
        data: form.serialize(),
        dataType: 'text',
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
        },
        success: function (response) {
            console.log("Success! submitAddAccountForm()");
            if (response == "Success") {
                SuccessNotification("Save Sucessful!", "Form submission was successful and data was saved!");
                GetAccountsView();
                UpdateTransactionManagerHead();
            }
            else {
                $("#createFormPartialWrapper").html(response);
                ErrorNotification("Error", "There was an error, please try again. If you continue to have problems please contact support.");
            }
        }
    })
}

//POST: Edit Account Submit Form
function submitEditAccountForm() {
    var form = $("#editAccountFormModalFormId");
    $.ajax({
        beforeSend: function () {
            $("#editModal-Body").html('<div class="" id="ajax-Accounts-loader1" style="text-align: center;"><div class="" style="display: inline-block;"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i></div></div>');
        },
        type: "Post",
        url: "/Saver/EditAccount",
        data: form.serialize(),
        dataType: 'text',
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
            $("#editModal-Body").html("<div class='text-center text-danger'><h4 class='text-danger'>Error when submitting form. Form: Category Edit</h4></div>");
        },
        success: function (response) {
            console.log("Success! submitEditCategoryForm()");
            if (response == "Success") {
                $("#editModal").modal('hide');
                SuccessNotification("Save Sucessful!", "Form submission was successful and data was saved!");
                GetAccountsListPartial();
            }
            else {
                $("#editModal-Body").html(response);
                ErrorNotification("Error", "There was an error, please try again. If you continue to have problems please contact support.");
            }
        }
    })
};
//Post: Toggle Account isActive State(true/false)
function toggleAccountStatusBtn(id, isActiveAction) {
    var rowElement = "#account-row-" + id;
    var dropdownElement = "#accountDropdown-" + id;
    $(dropdownElement).click();
    $.ajax({
        type: "Post",
        url: "/Saver/ActiveStatusToggleAccount",
        data: { id: id, isActiveAction: isActiveAction },
        dataType: 'text',
        error: function (response) {
            ErrorNotification("POST Error", "There was an error, please try again. If you continue to have problems please contact support.");
        },
        success: function (response) {
            console.log("Success!");
            if (response == "Success") {
                SuccessNotification("Save Sucessful!", "The request was successful and data was saved!");
                $(rowElement).remove();
            }
            else {
                ErrorNotification("Error", "There was an error, please try again. If you continue to have problems please contact support.");
            }
        }
    })
};

//########################################
//###### End Account Manage AJAX #########
//########################################