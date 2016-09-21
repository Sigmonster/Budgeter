

//########################################
//####### Start Account Manage AJAX ######
//########################################

//Get Account Data
function submitGetAccountForm() {
    console.log("submitGetAccountForm() Clicked")
    var id = $("#Accounts").val();
    var form = $("#AccountManageForm");

    $.ajax({
        beforeSend: function () {
            $("#displayAddTransactionWrapper").empty();
            $("#displayTransactionsWrapper").empty();
            $("#ajax-Accounts-loader1").show();
        },
        type: "Get",
        url: "/Saver/_AddTransactionPartial",
        data: form.serialize(),
        error: function (response) {
            console.log("Error _AddTransactionPartial! " + response);
        },
        success: function (response) {
            console.log("Success! _AddTransactionPartial");
            $("#displayAddTransactionWrapper").html(response);

        }
    });

    activeTransactions(id);

    $.ajax({
        type: "Get",
        url: "/Saver/_AccountManageHeadPartial",
        data: form.serialize(),
        error: function (response) {
            console.log("Error _AccountManageHeadPartial " + response);
        },
        success: function (response) {
            console.log("Success! _AccountManageHeadPartial");
            $("#ajax-Accounts-loader1").hide();
            $("#accountManageHeadWrapper").html(response);

        }
    });

}

//Get Active Transactions : _DisplayTransactionsPartial
function activeTransactions(accountId) {
    var id = parseInt(accountId);
    $.ajax({
        type: "Get",
        url: "/Saver/_DisplayTransactionsPartial",
        data: { accounts: id },
        error: function (response) {
            console.log("Error _DisplayTransactionsPartial! " + response);
        },
        success: function (response) {
            console.log("Success! _DisplayTransactionsPartial");
            //$("#ajax-Accounts-loader1").hide();
            $("#displayTransactionsWrapper").html(response);

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
            console.log("Error archivedTransactions(id)! " + archivedrRsponse);
        },
        success: function (archivedrRsponse) {
            console.log("Success! archivedTransactions(id)");
            $("#displayTransactionsWrapper").empty();
            $("#displayTransactionsWrapper").html(archivedrRsponse);
        }
    })
};

//Get Transaction To Edit(in Modal) : _EditTransactionPartial
function editTransactionBtn(id) {
    console.log("Edit Clicked, ID:" + id);
    var dropdownElement = "#transactionDropdown-" + id;
    $.ajax({
        beforeSend: function () {
            $(dropdownElement).click();
            $("#editTransactionModal-Body").empty();
            $("#editTransactionModal").modal('show');
        },
        type: "Get",
        url: "/Saver/EditTransaction",
        data: { id: id },
        dataType: 'text',
        error: function (response) {
            console.log("Error editTransactionBtn(id)! " + response);
        },
        success: function (response) {
            console.log("Success! _EditTransactionPartial");
            $("#editTransactionModal-Body").html(response);
        }
    })
};
//##########################
//##### START POST AJAX ####
//##########################
// Post Edit Transaction : _DisplayTransactionsPartial
function submitEditTransactionForm() {
    var form = $("#editTransactionModalFormId");
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    $.ajax({
        type: "POST",
        url: "/Saver/EditTransaction",
        data: form.serialize(),
        error: function (responseEditTransaction) {
            console.log("Error submitEditTransactionForm()! " + responseEditTransaction);
        },
        success: function (responseEditTransaction) {
            console.log("Success! submitEditTransactionForm()");
            $("#editTransactionModal-Body").empty();
            $("#editTransactionModal").modal('hide');
            $("#displayTransactionsWrapper").html(responseEditTransaction);
        }
    })
};
//Post Add Transation: _DisplayTransactionPartial, _AddTransactionPartial(Reset)
function submitAddTransactionForm() {
    var form = $("#addTransactionFormId");
    var token = $('input[name="__RequestVerificationToken"]', form).val();

    $.ajax({
        type: "POST",
        url: "/Saver/CreateTransaction",
        data: form.serialize(),
        error: function (responseAddTransaction) {
            console.log("Error submitEditTransactionForm()! " + responseAddTransaction);
        },
        success: function (responseAddTransaction) {
            console.log("Success! submitEditTransactionForm()");
            $("#displayTransactionsWrapper").html(responseAddTransaction);
        }
    })
};

// Post Void/Unvoid Transactions : _DisplayTransactionsPartial
function voidBtn(id, voidAction) {
    console.log("Clicked, ID:" + id);

    $.ajax({
        type: "POST",
        url: "/Saver/VoidTransaction",
        data: { id: id, voidAction: voidAction },
        dataType: 'text',
        error: function (response) {
            console.log("Error! " + response);
        },
        success: function (response) {
            console.log("Success!");
            $("#displayTransactionsWrapper").html(response);
        }
    })
};

// Post Proccessed/Pending Transactions :  _DisplayTransactionsPartial
function processedBtn(id, reconcileAction) {
    console.log("Clicked, ID:" + id);

    $.ajax({
        type: "POST",
        url: "/Saver/ReconcileTransaction",
        data: { id: id, reconcileAction: reconcileAction },
        dataType: 'text',
        error: function (response) {
            console.log("Error! " + response);
        },
        success: function (response) {
            console.log("Success!");
            $("#displayTransactionsWrapper").html(response);
        }
    })
};

// Post Active/InActive Transactive : _DisplayTransactionsPartial
// UI Reference Delete Transactions
function deleteBtn(id, isActiveAction) {
    console.log("Clicked, ID:" + id);

    $.ajax({
        type: "POST",
        url: "/Saver/ActiveStatusToggleTransaction",
        data: { id: id, isActiveAction: isActiveAction },
        dataType: 'text',
        error: function (response) {
            console.log("Error! " + response);
        },
        success: function (response) {
            console.log("Success!");
            $("#displayTransactionsWrapper").html(response);
        }
    })
};

//########################################
//##### END Account Manage AJAX ##########
//########################################

//########################################
//####### Start Budget Manage AJAX #######
//########################################

//Get Budget Add Partial : _BudgetAddPartial
function GetBudgetAddPartial() {
    console.log("submitGetAccountForm() Clicked")
    var form = $("#addBudgetForm");

    $.ajax({
        beforeSend: function () {
            $("#displayAddBudgetWrapper").empty();
        },
        type: "Get",
        url: "/Saver/_BudgetAddPartial",
        data: form.serialize(),
        error: function (response) {
            console.log("Error _BudgetAddPartial! " + response);
        },
        success: function (response) {
            console.log("Success! _BudgetAddPartial");
            $("#displayAddBudgetWrapper").html(response);

        }
    })
};
//Get Active Budgets Partial : _DisplayBudgetsPartial
function GetActiveBudgetsPartial() {
    console.log("GetActiveBudgetsPartial() Clicked")

    $.ajax({
        beforeSend: function () {
            $("#displayBudgetsWrapper").empty();
        },
        type: "Get",
        url: "/Saver/DisplayActiveBudgets",
        error: function (response) {
            console.log("Error GetActiveBudgetsPartial()! " + response);
        },
        success: function (response) {
            console.log("Success! GetActiveBudgetsPartial()");
            $("#displayBudgetsWrapper").html(response);

        }
    })
};
//Get Budget To Edit(in Modal) : _EditBudgetPartial
function editBudgetBtn(id) {
    console.log("Edit Clicked, ID:" + id);
    var dropdownElement = "#transactionDropdown-" + id;
    $.ajax({
        beforeSend: function () {
            $(dropdownElement).click();
            $("#editTransactionModal-Body").empty();
            $("#editTransactionModal").modal('show');
        },
        type: "Get",
        url: "/Saver/EditTransaction",
        data: { id: id },
        dataType: 'text',
        error: function (response) {
            console.log("Error editTransactionBtn(id)! " + response);
        },
        success: function (response) {
            console.log("Success! _EditTransactionPartial");
            $("#editTransactionModal-Body").html(response);
        }
    })
};
//##### Budget AJAX Posts
function submitAddBudgetForm() {
    console.log("Click! submitAddBudgetForm()");
    var form = $("#addBudgetForm");
    $.ajax({
        type: "POST",
        url: "/Saver/AddBudgetPartial",
        data: form.serialize(),
        dataType: 'text',
        error: function (response) {
            console.log("Error! submitAddBudgetForm()" + response);
        },
        success: function (response) {
            console.log("Success! submitAddBudgetForm()");
            $("#displayAddBudgetWrapper").load("/Saver/_BudgetAddPartial");
            GetActiveBudgetsPartial();
        }
    })
    
};

// Post Active/InActive Budget : _DisplayBudgetsPartial
// UI Reference Delete Transactions
function deleteBudgetBtn(id, isActiveAction) {
    console.log("Clicked, ID:" + id);

    $.ajax({
        type: "POST",
        url: "/Saver/ActiveStatusToggleBudget",
        data: { id: id, isActiveAction: isActiveAction },
        dataType: 'text',
        error: function (response) {
            console.log("Error! deleteBudgetBtn()" + response);
        },
        success: function (response) {
            console.log("Success! deleteBudgetBtn()");
            $("#displayBudgetsWrapper").html(response);
        }
    })
};

//########################################
//######## End Budget Manage AJAX ########
//########################################