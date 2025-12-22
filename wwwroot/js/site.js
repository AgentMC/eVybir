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