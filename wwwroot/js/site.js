// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function editCandidate(key, name, date, description, kind) {
    document.getElementById('candId').value = key;
    document.getElementById('candName').value = name;
    document.getElementById('candDate').value = date;
    document.getElementById('candDesc').value = description;
    document.getElementById('candKind').value = kind;
}
function editCampaign(key, name, dateStart, dateEnd) {
    document.getElementById('camId').value = key;
    document.getElementById('camName').value = name;
    document.getElementById('startDate').value = dateStart;
    document.getElementById('endDate').value = dateEnd;
}

function switchCampaign() {
    if (confirm("Зміни на цій сторінці не були збережені!\r\nПерейти на іншу виборчу подію?")) {
        var selectedCampaignId = document.getElementById('topCampaignSelector').value;
        location.href = selectedCampaignId;
    }
}
function filterCandidates() {
    var filter = $('input[type=radio]:checked').attr('data-filter');
    $('#leftSelect option').each((_, o) => {
        o = $(o);
        if (filter.includes(o.attr('data-kind'))) {
            o.show();
        } else {
            o.hide();
        }
    });
}

function updateButtonStates() {
    if ($('#leftSelect').prop('selectedIndex') > -1) {
        $('#btnAddParent').removeAttr('disabled');
    } else {
        $('#btnAddParent').attr('disabled',true);
    }
}

function addAsParent() {
    var leftSelect = $('#leftSelect')[0];
    var rightSelect = $('#rightSelect')[0];
    var hidden = $('#inclusionModel')[0]
    var model = JSON.parse(hidden.value);
    var nodeCount = leftSelect.selectedOptions.length;
    for (var i = 0; i < nodeCount; i++) {
        var option = leftSelect.selectedOptions[0];
        rightSelect.appendChild(option);
        model.push({
            "CandidateId": Number.parseInt(option.value),
            "DisplayOrder": model.length,
            "Children": []
        });
    }
    hidden.value = JSON.stringify(model);
}