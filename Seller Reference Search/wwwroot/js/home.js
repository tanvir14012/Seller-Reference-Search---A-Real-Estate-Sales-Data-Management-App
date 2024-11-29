$(function () {

    const lengthArr = [5, 10, 25, 50, 100];
    const acres = ['No Min', 1, 2, 5, 10, 20, 50, 100, 'No Max'];
    const prices = ['No Min', '50,000', '100,000', '150,000', '200,000', '250,000', '300,000', '350,000', '400,000', '450,000', '500,000', 'No Max'];

    $.fn.dataTable.ext.errMode = 'none';

    const tbl = $("#agents").DataTable({
        responsive: true,
        caption: 'Fig. Record of sales',
        layout: {
            topStart: 'search',
            top2Start: $(`
                    <div class="form-label">Lot size (minimum to maximum)</div>
                    <div class="row justify-content-between mx-auto mb-2">
                        <div class="h-35px border rounded py-1 px-2 small position-relative alt-dt-len-selectbox col-5" tabindex="0">
                            <div class="alt-dt-len-selectbox-arrow-down"></div>
                            <input type="text" readonly class="selected-val float-start invisible-input cursor-default" value="No Min" id="acre_min_input">
                            <ul class="alt-dt-len-select-dropdown list-group list-group-flush position-absolute border start-0 w-100" id="acre_min_ops">
                            </ul>
                        </div>
                        <div class="mx-2 text-muted mt-1 d-flex justify-content-center col-1 align-items-center">
                            <span>&#8212</span>
                        </div>
                        <div class="h-35px border rounded py-1 px-2 small position-relative alt-dt-len-selectbox col-5" tabindex="0">
                            <div class="alt-dt-len-selectbox-arrow-down"></div>
                            <input type="text" readonly class="selected-val float-start invisible-input cursor-default" value="No Max" id="acre_max_input">
                            <ul class="alt-dt-len-select-dropdown list-group list-group-flush position-absolute border start-0 w-100" id="acre_max_ops">
                            </ul>
                        </div>
                    </div>`),
            top2End: $(` <div class="float-left">
                    <div class="form-label">Offer price (minimum to maximum)</div>
                    <div class="row justify-content-between mx-auto">
                        <div class="h-35px border rounded py-1 px-2 small position-relative alt-dt-len-selectbox col-5" tabindex="0">
                            <div class="alt-dt-len-selectbox-arrow-down"></div>
                            <input type="text" readonly class="selected-val float-start invisible-input cursor-default" value="No Min" id="price_min_input">
                            <ul class="alt-dt-len-select-dropdown list-group list-group-flush position-absolute border start-0 w-100" id="price_min_ops">
                            </ul>
                        </div>
                            <div class="mx-2 text-muted mt-1 d-flex justify-content-center col-1 align-items-center">
                                <span>&#8212</span>
                            </div>
                        <div class="h-35px border rounded py-1 px-2 small position-relative alt-dt-len-selectbox col-5" tabindex="0">
                            <div class="alt-dt-len-selectbox-arrow-down"></div>
                            <input type="text" readonly class="selected-val float-start invisible-input cursor-default" value="No Max" id="price_max_input">
                            <ul class="alt-dt-len-select-dropdown list-group list-group-flush position-absolute border start-0 w-100" id="price_max_ops">
                            </ul>
                        </div>
                    </div></div>`),

            topEnd: $(`<div class="d-flex flex-column flex-md-row justify-content-between">
                <div class="mb-2 me-2 state-selection">
                    <div class="form-label">State</div>
                    <div class="h-35px border rounded py-1 px-2 small position-relative alt-dt-len-selectbox" tabindex="0">
                        <div class="alt-dt-len-selectbox-arrow-down"></div>
                        <input type="text" readonly class="selected-val float-start invisible-input cursor-default" value="Any" id="state_input">
                        <ul class="alt-dt-len-select-dropdown list-group list-group-flush position-absolute border start-0 w-100" id="state_dd">
                        </ul>
                    </div>
                </div>

                <div class="flex-grow-1 mb-2">
                    <div class="form-label">County</div>
                    <div class="h-35px border rounded py-1 px-2 small position-relative alt-dt-len-selectbox min-w-search-dropdown" tabindex="0">
                        <div class="alt-dt-len-selectbox-arrow-down"></div>
                        <input type="text" readonly class="selected-val float-start invisible-input cursor-default text-wrap me-3" value="Any" id="county_input">
                        <ul class="alt-dt-len-select-dropdown list-group list-group-flush position-absolute border start-0 w-100" id="county_dd">
                        </ul>
                    </div>
                </div>
            </div>`),

            top3End: {
                buttons: [
                    'colvis',
                    {
                        extend: 'copyHtml5',
                        text: '<i class="fas text-info fa-copy pe-1"></i>Copy',
                        filename: 'Sales',
                        title: 'Sales',
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
                        filename: 'Sales',
                        title: 'Sales',
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
                        filename: 'Sales',
                        title: 'Sales',
                        titleAttr: 'PDF',
                        pageSize: 'A3',
                        orientation: 'landscape',
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
                'DataTable_search',
                JSON.stringify(data)
            );

        },
        stateLoadCallback: (settings) => {

            const state = JSON.parse(localStorage.getItem('DataTable_search'));
            return state;
        },
        pageLength: 10,
        processing: true,
        serverSide: true,

        ajax: {
            url: '/home/index',
            type: 'POST',
            beforeSend: function () {
            },
            data: function (data) {
                try {
                    data["acreage_ref_max"] = searchOptions.max_acres;
                    data["offer_ref_max"] = searchOptions.offer_price_max;

                    //Check for initial state when UI components were not initialized yet
                    if (window.initCompleted) { //initialized
                        data["county"] = $("#county_input").val();
                        data["state"] = $("#state_input").val();
                        let acresMin = $("#acre_min_input").val();
                        let acresMax = $("#acre_max_input").val();
                        acresMin = acresMin.replace("No Min", "0");
                        acresMax = acresMax.replace("No Max", searchOptions.max_acres.toString());

                        data["acreage_min"] = acresMin;
                        data["acreage_max"] = acresMax;

                        let priceMin = $("#price_min_input").val().replace(",", "");
                        let priceMax = $("#price_max_input").val().replace(",", "");
                        priceMin = priceMin.replace("No Min", "0");
                        priceMax = priceMax.replace("No Max", searchOptions.offer_price_max.toString());
                        data["offer_min"] = priceMin;
                        data["offer_max"] = priceMax;
                    }
                    else { //not initialized

                        let customSettings = JSON.parse(localStorage.getItem("customSettings"));
                        customSettings = customSettings || {
                            "county": "Any",
                            "state": "Any",
                            "acreage_min": "0",
                            "acreage_max": searchOptions.max_acres.toString(),
                            "offer_min": "0",
                            "offer_max": searchOptions.offer_price_max.toString()
                        };

                        data["county"] = customSettings["county"];
                        data["state"] = customSettings["state"];

                        if (typeof customSettings["acreage_min"] !== 'undefined')
                            data["acreage_min"] = customSettings["acreage_min"].replace("No Min", "0");
                        else
                            data["acreage_min"] = "0";

                        if (typeof customSettings["acreage_max"] !== 'undefined')
                            data["acreage_max"] = customSettings["acreage_max"].replace("No Max", searchOptions.max_acres.toString());
                        else
                            data["acreage_max"] = searchOptions.max_acres.toString();

                        if (typeof customSettings["offer_min"] !== 'undefined')
                            data["offer_min"] = customSettings["offer_min"].replace("No Min", "0").replace(",", "");
                        else
                            data["offer_min"] = "0";

                        if (typeof customSettings["offer_max"] !== 'undefined')
                            data["offer_max"] = customSettings["offer_max"].replace("No Max", searchOptions.offer_price_max.toString()).replace(",", "");
                        else
                            data["offer_max"] = searchOptions.offer_price_max.toString();
                    }


                } catch {
                    console.log("Error reading filter settings: ");
                }
            }
        },
        columns: [
            { data: 'ownerName', searchable: true, orderable: true },
            { data: 'reference', searchable: true, orderable: true },
            {
                data: 'parcelNumber', searchable: true, orderable: true,
                render: function (data, type, row, meta) {
                    return `<i class="fas fa-file-alt me-1"></i>${data}`;
                }
            },
            {
                data: 'lotAcreage', searchable: false, orderable: true,
                render: function (data, type, row, meta) {
                    return `${data.toFixed(2)}`;
                }
            },
            { data: 'offerPrice', searchable: false, orderable: true, },
            { data: 'offerPPA', searchable: false, orderable: true, visible: false },
            { data: 'realPPA', searchable: false, orderable: true, visible: false },
            {
                data: 'ppaCalc', searchable: false, orderable: true, visible: false,
                render: function (data, type, row, meta) {
                    return `${data.toFixed(2)}`;
                }
            },
            { data: 'profit', searchable: false, orderable: true, visible: false },
            { data: 'retailValue', searchable: false, orderable: true, visible: false },
            { data: 'closingDate', searchable: false, orderable: true },
            { data: 'county', searchable: false, orderable: true, visible: false },
            { data: 'state', searchable: false, orderable: true, visible: false },
            { data: 'zipCode', searchable: false, orderable: true, visible: false },
            { data: 'lastModifiedAt', searchable: false, orderable: true, visible: false },
            {
                data: null,

                defaultContent: `
                    <ul class="list-group list-group-horizontal list-group-flush">
                        <li class="list-group-item p-0 me-2 rounded  mt-1">
                            <a onclick="viewRecord(event)" class="btn btn-light hover-bg-white" data-toggle="tooltip" title="Details">
                                <i class="fas fa-eye text-success"></i>
                                Details
                            </a>
                        </li>
                    </ul>`,
                orderable: false,
                className: 'action-btns',
                searchable: false
            }
        ],
        rowId: 'id',
        order: [[14, 'desc']],
        initComplete: (settings, json) => {

            //Read pagination state and update the custom select dropdown style
            const state = JSON.parse(localStorage.getItem('DataTable_search'));
            let dtLength = '10';
            if (state != null && typeof state !== 'undefined' && typeof state.length !== 'undefined' && state.length != null) {
                dtLength = state.length.toString();
            }
            $("#agents_wrapper div.dt-length .alt-dt-len-selectbox .selected-val").text(dtLength);
            $("#agents_wrapper div.dt-length .alt-dt-len-select-dropdown li").removeClass("selected");
            $("#agents_wrapper div.dt-length .alt-dt-len-select-dropdown li").each(function (li) {
                if (this.innerHTML === dtLength) {
                    $(this).addClass("selected");
                    return;
                }
            });

            //State and county dropdowns

            $("#state_dd").append(` <li class="list-group-item list-group-item-light px-2 py-0 border-0">Any</li>`);
            $.each(searchOptions.states, function (idx, val) {
                $("#state_dd").append(` <li class="list-group-item list-group-item-light px-2 py-0 border-0">${val}</li>`);
            });

            $("div.dt-search").parent().addClass("me-auto d-flex align-items-end mb-2 mt-4");

            $("#acre_min_input").closest(".col-md-auto").addClass("ms-auto ms-lg-0");
            $("#price_min_input").closest(".col-md-auto").addClass("me-auto me-lg-0");

            //Acres and price dropdowns
            loadAcreAndPriceDropdowns();

            $("#acre_min_ops").first().addClass("active");
            $("#acre_min_ops").last().addClass("active");

            $("#price_min_ops").first().addClass("active");
            $("#price_max_ops").last().addClass("active");

            window.initCompleted = true;

            loadCustomSettings();
        }

    });

    // Apply maxlength to the search input
    $('input[type=search]').attr('maxlength', '100');

    // Function to reload the table
    function reloadTable() {
        tbl.ajax.reload();
    }

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
            $("#agents_wrapper .alt-dt-len-select-dropdown").last().append(li);
        });
    }

    $(document).on("mouseenter mouseleave", ".alt-dt-len-select-dropdown li", function () {
        $("#agents_wrapper .alt-dt-len-select-dropdown li").removeClass("selected");
        $(this).addClass("selected");
    });

    $(document).on("click keydown", ".alt-dt-len-select-dropdown li", function (e) {

        const selectedVal = $(this).text();
        $(this).addClass("selected");

        $(e.currentTarget).parent().parent().children(".alt-dt-len-selectbox span.selected-val").text(selectedVal);
        $(e.currentTarget).parent().parent().children(".alt-dt-len-selectbox input.selected-val").val(selectedVal);

        const ulId = $(e.currentTarget).closest("ul.alt-dt-len-select-dropdown").attr("id");
        if (ulId === "acre_min_ops" || ulId === "price_min_ops") {
            loadAcreAndPriceDropdowns();
            const lotMin = (ulId === "acre_min_ops") ? selectedVal : $("#acre_min_input").val();
            const lotMax = $("#acre_max_input").val();
            const priceMin = (ulId === "price_min_ops") ? selectedVal: $("#price_min_input").val();
            const priceMax = $("#price_max_input").val();
            preserveMinMax(lotMin, lotMax, priceMin, priceMax);
        }

        saveCustomSettings(); //this exec order is important

        if (ulId === "acre_min_ops" || ulId === "price_min_ops" ||
            ulId === "acre_max_ops" || ulId === "price_max_ops") {
            reloadTable();
            return;
        }

        //State or county or othe dropdown change
        $(e.currentTarget).parent().parent().children(".alt-dt-len-selectbox input.selected-val").trigger("change");
        $(e.currentTarget).parent().parent().parent().children(".alt-dt-len-selectbox").trigger("blur");

        $(e.currentTarget).parent().parent().children(".alt-dt-len-select-dropdown li").removeClass("selected");

        // Set the datatable select value
        $(e.currentTarget).parent().parent().parent().parent().parent().children(".dt-length").children("select").val(selectedVal).trigger("change");
    });


    function closeDropdown(selectBox) {
        if ($(selectBox).find(".alt-dt-len-select-dropdown").hasClass("d-block")) {
            if (typeof window.dropdownSelectionStartTime !== 'undefined') {

                const diff = new Date().getTime() - window.dropdownSelectionStartTime;

                //Close the dropdown on 2nd click
                if (diff > 10) {
                    $(selectBox).find('.alt-dt-len-select-dropdown').removeClass("d-block");
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


        $(selectBox).find('.alt-dt-len-select-dropdown').addClass("d-block");
        $(selectBox).find('.alt-dt-len-selectbox-arrow-down').css({
            'transform': 'rotate(45deg)',
            'top': '13px'
        });
    }

    //Keep focus/unfocus time of select dropdowns
    $(document).on("focus", ".alt-dt-len-selectbox", function (e) {
        window.dropdownSelectionStartTime = new Date().getTime();
    });

    $(document).on("blur", ".alt-dt-len-selectbox", function (e) {
        const $currentTarget = $(e.currentTarget);

        //When you click an item in the dropdown, both`click` and `blur` events occur almost simultaneously.Without`setTimeout`,
        //the`blur` event might close the dropdown before the `click` event is fully processed, disrupting event listeners.
        setTimeout(function () {
            $currentTarget.find(".alt-dt-len-select-dropdown")
                .removeClass("d-block");

            $currentTarget.find('.alt-dt-len-selectbox-arrow-down').css({
                'transform': '',
                'top': ''
            });
        }, 150);
    });

    //dt select's wrapper dropdown(dd)
    $(document).on("click", ".alt-dt-len-selectbox", function (e) {

        let selectedVal = $(e.currentTarget).find("input.selected-val").val();
        if (typeof selectedVal === 'undefined' || selectedVal === null) {
            selectedVal = $(e.currentTarget).find("span.selected-val").text();
        }

        $(e.currentTarget).find(".alt-dt-len-select-dropdown li").removeClass("selected");
        $(e.currentTarget).find(".alt-dt-len-select-dropdown li").each(function (li) {
            if (this.innerHTML === selectedVal) {
                $(this).addClass("selected");
                return;
            }
        });

         
        let selectBox = $(this);
        closeDropdown(selectBox);

    });


    const settings = JSON.parse(localStorage.getItem('customSettings')) || {};


    //Reload table on the dropdown changes
    $(document).on("change", ".alt-dt-len-selectbox input.selected-val", function (evt) {

        //If state is changed
        if ($(evt.currentTarget).attr('id') === "state_input") {
            const state = $(evt.currentTarget).val();
            const countySelectionBefore = $("#county_input").val();
            $("#county_input").val('');

            new Promise((resolve, reject) => {
                $.ajax({
                    url: '/home/getCounties',
                    method: 'POST',
                    contentType: "application/json; charset=utf-8",
                    data: JSON.stringify(state),
                    success: resolve,
                    error: reject
                }).then(resp => {
                    const counties = ['Any'].concat(resp);
                    let selectedClass = "";

                    $("#county_dd li").remove();
                    $.each(counties, function (idx, county) {
                        if (county === countySelectionBefore) {
                            selectedClass = "selected";
                        }
                        else {
                            selectedClass = "";
                        }
                        let el = `<li class="list-group-item list-group-item-light px-2 py-0 border-0 ${selectedClass}">${county}</li>`;
                        $("#county_dd").append(el);

                        if (idx === counties.length - 1) {

                            if (selectedClass === "") {
                                $("#county_dd li").first().addClass("selected");
                                $("#county_input").val($("#county_dd li").first().text());
                            }
                            else {
                                $("#county_input").val(countySelectionBefore);
                            }
                        }

                    });

                    saveCustomSettings();

                    reloadTable();

                }).catch(err => {
                      console.log('Error. Failed to load counties!');  
                    });  
                    
            }); //Promise end
        }
        else {
            //Reload the data table since county is changed
            reloadTable();
        }

    });

    // When the data table is redrawn because of events such as pagination, reload, search, sort etc.
    tbl.on('draw', () => {

    });


    window.viewRecord = (e) => {
        let id = $(e.target).closest("tr").attr("id");
        if (typeof id === 'undefined') {
            id = $(e.target).closest("tr").prev("tr").attr("id");
        }

        if (id) {
            location.href = `/home/details?id=${id}`;
        }
    }

    function saveCustomSettings() {
        let customSettings = {};
        try {
            customSettings = JSON.parse(localStorage.getItem("customSettings"));
            customSettings = customSettings || {};
        } catch { }

        customSettings["county"] = $("#county_dd").parent().find("input.selected-val").val();
        customSettings["state"] = $("#state_dd").parent().find("input.selected-val").val();
        customSettings["county_ops"] = $("#county_dd li").map((_, li) => $(li).text()).toArray();
        let acresMin = $("#acre_min_input").val();
        let acresMax = $("#acre_max_input").val();

        let priceMin = $("#price_min_input").val();
        let priceMax = $("#price_max_input").val();

        customSettings["acreage_min"] = acresMin;
        customSettings["acreage_max"] = acresMax;
        customSettings["offer_min"] = priceMin;
        customSettings["offer_max"] = priceMax;

        localStorage.setItem('customSettings', JSON.stringify(customSettings));
    }

    function loadCustomSettings() {
        try {
            let customSettings = JSON.parse(localStorage.getItem('customSettings'));
            if (typeof customSettings === 'undefined') {
                saveCustomSettings();
            }

            if (typeof customSettings["county"] === 'undefined')
                customSettings["county"] = "Any";
            if (typeof customSettings["state"] === 'undefined')
                customSettings["state"] = "Any";

            $("#county_input").val(customSettings["county"]);
            $("#state_input").val(customSettings["state"]);

            let counties = customSettings["county_ops"];
            if (typeof counties === 'undefined')
                counties = ['Any'];

            if (typeof customSettings["acreage_min"] === 'undefined')
                customSettings["acreage_min"] = "No Min";

            if (typeof customSettings["acreage_max"] === 'undefined')
                customSettings["acreage_max"] = "No Max";

            if(typeof customSettings["offer_min"] === 'undefined') 
                customSettings["offer_min"] = "No Min";

            if (typeof customSettings["offer_max"] === 'undefined')
                customSettings["offer_max"] = "No Max";

            $("#county_dd li").remove();
            $.each(counties, (idx, c) => {
                $("#county_dd").append(`<li class="list-group-item list-group-item-light px-2 py-0 border-0">${c}</li>`);
            });
            $("#acre_min_input").val( customSettings["acreage_min"]);
            $("#acre_max_input").val( customSettings["acreage_max"]);
            $("#price_min_input").val(customSettings["offer_min"]  );
            $("#price_max_input").val(customSettings["offer_max"]);

            preserveMinMax(
                customSettings["acreage_min"]
                , customSettings["acreage_max"]
                , customSettings["offer_min"]  
                , customSettings["offer_max"]
            );
        } catch { }
    }

    function loadAcreAndPriceDropdowns() {

        $("#acre_min_ops li").remove();
        $("#acre_max_ops li").remove();
        $("#price_min_ops li").remove();
        $("#price_max_ops li").remove();

        $.each(acres, function (idx, val) {

            if (idx != acres.length - 1)
                $("#acre_min_ops").append(` <li class="list-group-item list-group-item-light px-2 py-0 border-0" data-idx="${idx}">${val}</li>`);
            if (idx != 0)
                $("#acre_max_ops").append(` <li class="list-group-item list-group-item-light px-2 py-0 border-0" data-idx="${idx}">${val}</li>`);
        });

        $.each(prices, function (idx, val) {
            if (idx != prices.length - 1)
                $("#price_min_ops").append(` <li class="list-group-item list-group-item-light px-2 py-0 border-0" data-idx="${idx}">${val}</li>`);
            if (idx != 0)
                $("#price_max_ops").append(` <li class="list-group-item list-group-item-light px-2 py-0 border-0" data-idx="${idx}">${val}</li>`);
        });
    }

    function preserveMinMax(lotMin, lotMax, priceMin, priceMax) {

        //Acre

        const lotMinSelectedIndex = $("#acre_min_ops li")
            .filter((_, el) => $(el).text() === lotMin)
            .eq(0)
            .attr("data-idx");

       
        const lotMaxSelectedIndex = $("#acre_max_ops li")
            .filter((_, el) => $(el).text() === lotMax)
            .eq(0)
            .attr("data-idx");

        const selectedVal1 = $("#acre_max_ops li").eq(parseInt(lotMinSelectedIndex)).text();

        if (lotMinSelectedIndex !== undefined) {
            $("#acre_max_ops li").filter((_, el) => $(el).attr("data-idx") <= lotMinSelectedIndex)
                .remove();
        }

        if (lotMinSelectedIndex !== undefined && lotMaxSelectedIndex !== undefined) {
            if (parseInt(lotMinSelectedIndex) >= parseInt(lotMaxSelectedIndex)) {
                $("#acre_max_ops li").removeClass("selected");
                $("#acre_max_ops li").eq(parseInt(lotMinSelectedIndex)).addClass("selected");
                $("#acre_max_input").val(selectedVal1);
            }
        }

        //Offer price
        const priceMinSelectedIndex = $("#price_min_ops li")
            .filter((_, el) => $(el).text() === priceMin)
            .eq(0)
            .attr("data-idx");

        const priceMaxSelectedIndex = $("#price_max_ops li")
            .filter((_, el) => $(el).text() === priceMax)
            .eq(0)
            .attr("data-idx");

        const selectedVal2 = $("#price_max_ops li").eq(parseInt(priceMinSelectedIndex)).text();

        if (priceMinSelectedIndex !== undefined) {
            $("#price_max_ops li").filter((_, el) => $(el).attr("data-idx") <= priceMinSelectedIndex)
                .remove();
        }

        if (priceMinSelectedIndex !== undefined && priceMaxSelectedIndex !== undefined) {
            if (parseInt(priceMinSelectedIndex) >= parseInt(priceMaxSelectedIndex)) {
                $("#price_max_ops li").removeClass("selected");
                $("#price_max_ops li").eq(parseInt(priceMinSelectedIndex)).addClass("selected");
                $("#price_max_input").val(selectedVal2);
            }
        }
    }



});