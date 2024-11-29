$(function () {

    const lengthArr = [5, 10, 25, 50, 100];

    $.fn.dataTable.ext.errMode = 'none';  

    const tbl = $("#agents").DataTable({
        responsive: true,
        caption: 'Fig. Record of file uploads',
        layout: {
            topStart: 'search',
            topEnd: {
                buttons: [
                    'colvis',
                    {
                        extend: 'copyHtml5',
                        text: '<i class="fas text-info fa-copy pe-1"></i>  Copy',
                        filename: 'File uploads',
                        title: 'File uploads',
                        titleAttr: 'Copy to Clipboard',
                        exportOptions: {
                            modifier: {
                                page: 'current'
                            },
                            columns: ':visible'
                        }
                    },
                    {
                        extend: 'excelHtml5',
                        text: '<i class="fas text-info fa-file-excel pe-1"></i>  Excel',
                        filename: 'File uploads',
                        title: 'File uploads',
                        titleAttr: 'Export to Excel',
                        exportOptions: {
                            modifier: {
                                page: 'current'
                            },
                            columns: ':visible'
                        }
                    },
                    {
                        extend: 'pdfHtml5',
                        text: '<i class="fas fa-file-pdf text-info pe-1"></i>  PDF',
                        filename: 'File uploads',
                        title: 'File uploads',
                        titleAttr: 'PDF',
                        pageSize: 'A3',
                        exportOptions: {
                            modifier: {
                                page: 'current'
                            },
                            columns: ':visible'
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

                            // Set specific column widths
                            doc.content[1].table.widths = [
                                '30%', // First column width
                                '10%', // Second column width
                                '50%', // Third column width
                            ];
                            

                            // Align all cells to the left and vertically middle
                            doc.content[1].table.body.forEach(function (row) {
                                row.forEach(function (cell) {
                                    cell.alignment = 'left'; // Left align all cells
                                    cell.margin = [0, 5, 0, 5]; // Vertically middle
                                });
                            });
                        },
                    }
                ]
            },
            bottomStart: 'pageLength'
        },
        lengthMenu: lengthArr,
        stateSave: true,
        stateSaveCallback: (settings, data) => {
            localStorage.setItem(
                'DataTable_uploads',
                JSON.stringify(data)
            );
        },
        stateLoadCallback: (settings) => {
            const state = JSON.parse(localStorage.getItem('DataTable_uploads'));
            return state;
        },
        processing: true,
        serverSide: true,
        pageLength: 10,
        ajax: {
            url: '/upload/getUploads',
            type: 'POST'
        },
        columns: [
            { data: 'fileName', searchable: true, orderable: true },
            { data: 'status', searchable: true, orderable: true },
            { data: 'description', searchable: true, orderable: true },
            { data: 'createdAt', searchable: false, orderable: true },
            {
                data: null,
                defaultContent: `
                    <ul class="list-group list-group-horizontal list-group-flush">
                        <li class="list-group-item p-0 me-2 rounded">
                            <a onclick="deleteRecord(event)" class="btn btn-light hover-bg-white" data-toggle="tooltip" title="Delete">
                                <i class="fas fa-trash text-danger"></i>
                            </a>
                        </li>
                    </ul>`,
                orderable: false,
                className: 'delete-btn-wrapper',
                searchable: false,
                visible: false
            }

        ],
        rowId: 'id',
        order: [[3, 'desc']],
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
            const state = JSON.parse(localStorage.getItem('DataTable_uploads'));
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

        } //Init complete closing brace


    });

    //Hide/show the delete button based on the admin role
    if (window.isAdmin === "True") {
        tbl.column(4).visible(true);
    }

    // Listen for column visibility changes
    tbl.on('column-visibility.dt', function (e, settings, column, state) {
        console.log('Column index changed: ' + column);
        console.log('New visibility state: ' + state);
        // You can add your custom logic here
    });

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
            $("#agents_wrapper .dt-buttons").first().find("button").first().show();
        }
        else
            $("#agents_wrapper .dt-buttons").first().find("button").first().hide();
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

    clearForm = function () {
        $("#uploadForm input[type=file]").val('');
        $("#agent_wrapper div.alert").alert('close'); 
        if (window.history.replaceState) {
            window.history.replaceState(null, null, window.location.href);
        }
        location.reload();
    }

    window.deleteRecord = (e) => {
        const id = $(e.currentTarget).closest("tr").attr("id");
        handleDelete([parseInt(id)], tbl);
    };

    const sendDeleteRequest = (ids) => {
        return new Promise((resolve, reject) => {
            $.ajax({
                url: '/upload/deleteUploads',
                method: 'POST',
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(ids),
                success: resolve,
                error: reject
            });
        });
    }

    const handleDelete = (ids, dt) => {
        const response = confirm("Are you sure you want to delete the uploaded file and the associated records? This action cannot be undone.");
        if (response) {
            sendDeleteRequest(ids)
                .then(resp => {
                    dt.ajax.reload(null, false);
                    $("#agents_wrapper").find("tr.selected").removeClass("selected");
                    alert('The upload has been deleted successfully!');
                })
                .catch(err => {
                    alert('Error! The upload has not been deleted.');
                });
        }
    };

    /**
     * Sheet validation
     
    <!-- use xlsx.full.min.js from version 0.20.3 -->
    <script lang="javascript" src="https://cdn.sheetjs.com/xlsx-0.20.3/package/dist/xlsx.full.min.js"></script>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap-select@1.13.14/dist/css/bootstrap-select.min.css">

    <!-- Latest compiled and minified JavaScript -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-select@1.13.14/dist/js/bootstrap-select.min.js"></script>

    <!-- (Optional) Latest compiled and minified JavaScript translation files -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap-select@1.13.14/dist/js/i18n/defaults-*.min.js"></script>

    $('input[type=file]').on("change", function (e) {
        const files = e.target.files;
        if (files.length === 0) {
            return;
        }

        const file = files[0];
        console.log(file);
        if (file) {
            const size = file.size / (1024 * 1024);
            if (size <= 5.0) {

                const reader = new FileReader();
                reader.onload = function (evt) {
                    const data = new Uint8Array(evt.target.result);
                    const wb = XLSX.read(data, {
                        type: "array",
                        sheetRows: 1
                    });
                    console.log(wb);
                    const wsNames = wb["SheetNames"];
                    if (wsNames.length > 0) {
                        const ws = wb.Sheets[wsNames[0]];
                        console.log(ws);
                        var range = XLSX.utils.decode_range(ws['!ref']);
                        var ncols = range.e.c - range.s.c + 1, nrows = range.e.r - range.s.r + 1;
                        console.log(range);
                        console.log(ncols);

                        // Convert the first row (header) to JSON
                        const sheetData = XLSX.utils.sheet_to_json(ws, { header: 1 });
                        const headerRow = sheetData[0];
                        console.log(headerRow);
                    }
                }

                reader.readAsArrayBuffer(file);



                console.log(wb);
            }
        }

    });
    **/


});