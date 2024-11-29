$(function () {

    const lengthArr = [5, 10, 25, 50, 100];

    $.fn.dataTable.ext.errMode = 'none';  

    const tbl = $("#agents").DataTable({
        responsive: true,
        caption: 'Fig. Record of app users',
        layout: {
            topStart: 'search',
            topEnd: {
                buttons: [
                    'colvis',
                    {
                        extend: 'copyHtml5',
                        text: '<i class="fas text-info fa-copy pe-1"></i>  Copy',
                        filename: 'App users',
                        title: 'App users',
                        titleAttr: 'Copy to Clipboard',
                        exportOptions: {
                            modifier: {
                                page: 'current'
                            },
                            columns: ':not(:last-child)'
                        }
                    },
                    {
                        extend: 'excelHtml5',
                        text: '<i class="fas text-info fa-file-excel pe-1"></i>  Excel',
                        filename: 'App users',
                        title: 'App users',
                        titleAttr: 'Export to Excel',
                        exportOptions: {
                            modifier: {
                                page: 'current'
                            },
                            columns: ':not(:last-child)'
                        }
                    },
                    {
                        extend: 'pdfHtml5',
                        text: '<i class="fas fa-file-pdf text-info pe-1"></i>  PDF',
                        filename: 'App users',
                        title: 'App users',
                        titleAttr: 'PDF',
                        orientation: 'potrait',
                        pageSize: 'A5',
                        exportOptions: {
                            modifier: {
                                page: 'current'
                            },
                            columns: ':not(:last-child)'
                        },
                        customize: function (doc) {

                            // Align the title to the left
                            doc.styles.title = {
                                alignment: 'left'
                            };

                            // Styles for table headers
                            doc.styles.tableHeader = {
                                alignment: 'left', // Align headers to the left
                                color: 'black',
                                background: 'white'
                            };

                            //// Adjust column widths
                            //doc.content[1].table.widths =
                            //    Array(doc.content[1].table.body[0].length + 1).join('*').split('');

                            // Align all cells to the left and vertically middle
                            doc.content[1].table.body.forEach(function (row) {
                                row.forEach(function (cell) {
                                    cell.alignment = 'left'; // Left align all cells
                                    cell.margin = [0, 5, 0, 5]; // Vertically middle
                                });
                            });
                        },
                    },
                    {
                        text: 'Delete Selected',
                        action: (e, dt) => {
                            const selectedRowIds = Array.from($(e.currentTarget).closest("#agents_wrapper")
                                .find("tbody tr.selected"))
                                .map(el => parseInt(el.id));
                            handleDelete(selectedRowIds, dt);
                        },
                        init: function (api, node, config) {
                            $(node).attr('title', 'Administrator users will not be deleted');
                        }
                    },
                    {
                        text: 'Delete All',
                        action: (e, dt) => handleDelete([-1], dt),
                        init: function (api, node, config) {
                            $(node).attr('title', 'Administrator users will not be deleted');
                        }
                    }
                ]
            },
            bottomStart: 'pageLength'
        },
        lengthMenu: lengthArr,
        stateSave: true,
        stateSaveCallback: (settings, data) => {
            localStorage.setItem(
                'DataTable_users',
                JSON.stringify(data)
            );
        },
        stateLoadCallback: (settings) => {
            const state = JSON.parse(localStorage.getItem('DataTable_users'));
            return state;
        },
        processing: true,
        serverSide: true,
        pageLength: 10,
        ajax: {
            url: '/team/getUsers',
            type: 'POST'
        },
        columns: [
            {
                data: null,
                defaultContent: '',
                orderable: false,
                className: 'select-checkbox',
                searchable: false,
                visible:false
            },
            { data: 'FirstName', searchable: true },
            { data: 'LastName', searchable: true },
            { data: 'Email', searchable: true },
            {
                data: null,
                defaultContent: `
                    <ul class="list-group list-group-horizontal list-group-flush">
                        <li class="list-group-item p-0 me-2 rounded">
                            <a onclick="editRecord(event)" class="btn btn-light hover-bg-white" data-toggle="tooltip" title="Edit">
                                <i class="fas fa-edit text-info"></i>
                            </a>
                        </li>
                        <li class="list-group-item p-0 me-2 rounded">
                            <a onclick="deleteRecord(event)" class="btn btn-light hover-bg-white" data-toggle="tooltip" title="Delete">
                                <i class="fas fa-trash text-danger"></i>
                            </a>
                        </li>
                    </ul>`,
                orderable: false,
                className: 'action-btns',
                searchable: false
            }
        ],
        rowId: 'Id',
        order: [[1, 'asc']],
        select: {
            style: 'multi',
            selector: 'td:first-child'
        },
        initComplete: (settings, json) => {
            // Add name to the first menu item of the column visibility button's dropdown menu
            $("#agents_wrapper .dt-buttons button.dropdown-toggle").on("click", () => {
                $("#agents_wrapper .dt-buttons .dropdown-menu a:nth-child(1) > span").text("Checkbox");
            });

            //The page size select element is replaced with a custom styled dropdown
            let customSelectTemplate = `
                <div class="d-flex">
                    <div class="w-65px h-35px border rounded py-1 px-2 small position-relative alt-dt-len-selectbox" tabindex="0">
                        <div class="alt-dt-len-selectbox-arrow-down"></div>
                        <span class="selected-val float-start">${lengthArr[0]}</span>
                        <ul class="alt-dt-len-select-dropdown list-group list-group-flush position-absolute border w-65px start-0">
                            <li class="list-group-item list-group-item-light px-2 py-0 border-0 selected">${lengthArr[0]}</li>
                        </ul>
                    </div>
                    <div class="px-2 py-1 text-break"> entries per page</div>
                </div>`;

            $("#agents_wrapper .dt-length").children().addClass("visually-hidden");
            $("#agents_wrapper .dt-length").append(customSelectTemplate);
            if (lengthArr.length > 1) {
                lengthArr.slice(1).forEach(len => {
                    let li = `<li class="list-group-item list-group-item-light px-2 py-0 border-0">${len}</li>`;
                    $("#agents_wrapper .alt-dt-len-select-dropdown").append(li);
                });
            }

            //Read pagination state and update the custom select dropdown style
            const state = JSON.parse(localStorage.getItem('DataTable_users'));
            $(".alt-dt-len-selectbox .selected-val").text(state.length);
            $("#agents_wrapper .alt-dt-len-select-dropdown li").removeClass("selected");
            $("#agents_wrapper .alt-dt-len-select-dropdown li").each(function (li) {
                if (this.innerHTML === state.length.toString()) {
                    $(this).addClass("selected");
                    return;
                }
            });

            $("#agents_wrapper .alt-dt-len-select-dropdown li").on("mouseenter mouseleave", function () {
                $("#agents_wrapper .alt-dt-len-select-dropdown li").removeClass("selected");
                $(this).addClass("selected");
            });

            $("#agents_wrapper .alt-dt-len-select-dropdown li").on("click", function () {
                const selectedVal = $(this).text().trim();
                $(".alt-dt-len-selectbox .selected-val").text(selectedVal);

                $("#agents_wrapper .alt-dt-len-select-dropdown li").removeClass("selected");
                $(this).addClass("selected");

                // Set the datatable select value
                $("#agents_wrapper .dt-length select").val(selectedVal).trigger("change");
            });

            // Hide the 'Delete Selected' button initially
            $("#agents_wrapper .dt-buttons").find("button").eq(4).hide();

            //Keep focus/unfocus time of select dropdowns
            $(document).on("focus", ".alt-dt-len-selectbox", function (e) {
                window.dropdownSelectionStartTime = new Date().getTime();
            });

            $(document).on("blur", ".alt-dt-len-selectbox", function (e) {
                window.dropdownSelectionEndTime = new Date().getTime();
                $(e.currentTarget).find('.alt-dt-len-select-dropdown').css('display', 'none');
                $(e.currentTarget).find('.alt-dt-len-selectbox-arrow-down').css({
                    'transform': '',
                    'top': ''
                });
            });

            //Fix dropdown selected item's hightlight
            $(".alt-dt-len-selectbox").on("click", function (e) {
                const selectedVal = $(e.currentTarget).find("span.selected-val").text();
                console.log(selectedVal);
                $(e.currentTarget).find(".alt-dt-len-select-dropdown li").removeClass("selected");
                $(e.currentTarget).find(".alt-dt-len-select-dropdown li").each(function (li) {
                    if (this.innerHTML === selectedVal) {
                        $(this).addClass("selected");
                        return;
                    }
                });

                const selectBox = $(this);

                if ($(selectBox).find(".alt-dt-len-select-dropdown").css("display") === "block") {
                    if (typeof window.dropdownSelectionStartTime !== 'undefined') {

                        const diff = new Date().getTime() - window.dropdownSelectionStartTime;

                        //Close the dropdown on 2nd click
                        if (diff > 10) {
                            $(selectBox).find('.alt-dt-len-select-dropdown').css('display', 'none');
                            $(selectBox).find('.alt-dt-len-selectbox-arrow-down').css({
                                'transform': '',
                                'top': ''
                            });
                            return;
                        }
                        else {

                            $(selectBox).find('.alt-dt-len-selectbox-arrow-down').css({
                                'transform': '',
                                'top': ''
                            });
                        }
                    }

                }


                $(selectBox).find('.alt-dt-len-select-dropdown').css('display', 'block');
                $(selectBox).find('.alt-dt-len-selectbox-arrow-down').css({
                    'transform': 'rotate(45deg)',
                    'top': '13px'
                });

            });
        }
    });

    // Apply maxlength to the search input
    $('input[type=search]').attr('maxlength', '100');
    tbl.on('click', 'thead tr th.select-checkbox', function (e) {
        const parentTr = $(e.currentTarget).parent("tr");
        parentTr.toggleClass("selected");

        if (parentTr.hasClass("selected")) {
            parentTr.closest("table").children("tbody").children("tr").addClass("selected");
        } else {
            parentTr.closest("table").children("tbody").children("tr").removeClass("selected");
        }

        // Toggle Delete Selected button
        toggleDeleteSelectedBtn();
    });

    // Toggle 'Delete Selected' button
    const toggleDeleteSelectedBtn = () => {
        const selectedItemCount = $("#agents_wrapper table tbody").children("tr.selected").length;

        if (selectedItemCount > 0) {
            $("#agents_wrapper .dt-buttons").find("button").eq(4).show();
        }
        else
            $("#agents_wrapper .dt-buttons").find("button").eq(4).hide();
    }

    // Handle class change in tbody > tr
    const classChangeObserverCallback = (mutationList) => {
        for (const mutation of mutationList) {
            if (mutation.type === 'attributes' && mutation.attributeName === 'class') {
                toggleDeleteSelectedBtn();
            }
        }
    }

    let tlbRowClassChangeObservers = [];

    // When the data table is redrawn because of events such as pagination, reload, search, sort etc.
    tbl.on('draw', () => {
        // Destroy previous observers
        for (const observer of tlbRowClassChangeObservers) {
            observer.disconnect();
        }
        tlbRowClassChangeObservers = [];

        const allTblRows = document.querySelectorAll("#agents_wrapper table tbody tr");

        for (const tr of allTblRows) {
            const observer = new MutationObserver(classChangeObserverCallback);

            observer.observe(tr, {
                attributes: true
            });

            tlbRowClassChangeObservers.push(observer);
        }
    });

    const sendDeleteRequest = (ids) => {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: '/team/deleteUsers',
                method: 'POST',
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(ids),
                success: resolve,
                error: reject
            });
        });
    }

    window.editRecord = (e) => {
        bsFormWrapper.show();
        const model = tbl.row($(e.target).closest("tr")).data();

        //Set input field values
        $("#agentForm input:hidden[name='Id']").val(model["Id"]);
        $("#agentForm input[name='FirstName']").val(model["FirstName"]);
        $("#agentForm input[name='LastName']").val(model["LastName"]);
        $("#agentForm input[name='Email']").val(model["Email"]);

        $("#agentsRoot")[0].scrollIntoView({ behavior: 'smooth' });
    }

    window.deleteRecord = (e) => {
        const id = $(e.currentTarget).closest("tr").attr("id");
        handleDelete([parseInt(id)], tbl);
    };

    const handleDelete = (ids, dt) => {
        const response = confirm("Are you sure you want to delete the record(s)? This action cannot be undone.");
        if (response) {
            sendDeleteRequest(ids)
                .then(resp => {
                    dt.ajax.reload(null, false);
                    $("#agents_wrapper").find("tr.selected").removeClass("selected");
                    alert('The record(s) has been deleted successfully!');
                })
                .catch(err => {
                    alert('Error! The record(s) has not been deleted.');
                });
        }
    };

    // Init the collapse object for (hide/show)
    const bsFormWrapper = new bootstrap.Collapse($("#agentFormWrapper"), {
        toggle: false
    });

    const clearAgentForm = () => {

        // Remove validation messages
        $("#agentForm").find("[data-valmsg-summary=true]")
            .removeClass("validation-summary-errors")
            .addClass("validation-summary-valid")
            .find("ul").empty();

        $("#agentForm").find("[data-valmsg-replace]")
            .removeClass("field-validation-error")
            .addClass("field-validation-valid")
            .empty();

        const valdtnSmryWrpr = $("#agentForm > div[data-valmsg-summary]");
        valdtnSmryWrpr.addClass("validation-summary-valid")
            .removeClass("validation-summary-errors");

        $(":input", "#agentForm")
            .not(":button, :submit, :reset, :hidden")
            .val("")
            .prop("checked", false)
            .prop("selected", false);
    }

    $("#agentFormWrapper").on("show.bs.collapse", () => {
        // Enable unobtrusive validation for the form, otherwise the form will get submitted on the collapsible UI toggle
        $.validator.unobtrusive.parse($("#agentForm"));

        clearAgentForm();

        $("div[data-valmsg-summary] ul").addClass("mx-auto");

        $("#addBtn > span").text("Close");
        $("#addBtn > i").removeClass("fa-plus").addClass("fa-xmark");
    });

    $("#agentFormWrapper").on("hidden.bs.collapse", () => {
        $('#agentForm').trigger("reset");
        $("#agentForm").removeData("validator")    // Remove data-* states added by jQuery Validation
            .removeData("unobtrusiveValidation"); // Remove data-* states added by jQuery Unobtrusive Validation

        $("#addBtn > span").text("Add");
        $("#addBtn > i").removeClass("fa-xmark").addClass("fa-plus");
    });

    // Save
    $("#agentsRoot #saveBtn").on("click", () => {

        //Check if two passwords match
        if ($("#pass").val() !== $("#confirmPass").val()) {
            $("#confirmPassErrMsg").removeClass("invisible");
            return;
        }
        else {
            $("#confirmPassErrMsg").addClass("invisible");
        }

        const form = $("#agentForm");
        if (form.valid()) {
            $.ajax({
                type: form.attr("method"),
                url: form.attr("action"),
                data: form.serialize(),
                headers: {
                    "X-CSRF-TOKEN": $('#agentForm input:hidden[name="__RequestVerificationToken"]').val()
                },
                success: (resp) => {
                    clearAgentForm();
                    tbl.ajax.reload(null, false);
                    clearAgentForm();
                    alert('Success! The record is saved');
                },
                error: (err) => {
                    const errList = err.responseJSON?.errors;
                    if (Array.isArray(errList)) {

                        const valdtnSmryWrpr = $("#agentForm > div[data-valmsg-summary]");
                        valdtnSmryWrpr.removeClass("validation-summary-valid")
                            .addClass("validation-summary-errors");

                        errList.forEach(errMsg => {
                            
                            const li = `<li>${errMsg}</li>`;
                            $(valdtnSmryWrpr).children("ul").first().append(li);
                        });
                    }
                    alert('Error! The record could not be saved.');
                }
            });
        }
    });

    //Password and confirm password - eye toggle
    $("div.position-relative > i.position-absolute").on("click", function (evt) {
        const target = $(evt.currentTarget);
        const inputField = target.siblings("input");

        if (target.hasClass("fa-eye")) {
            target.removeClass("fa-eye").addClass("fa-eye-slash");
            inputField.attr("type", "text");
        } else if (target.hasClass("fa-eye-slash")) {
            target.removeClass("fa-eye-slash").addClass("fa-eye");
            inputField.attr("type", "password");
        }
    });


});