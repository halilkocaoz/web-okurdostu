﻿var validatetoken = $("input[name=__RequestVerificationToken]").val();
const Toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    timer: 2000,
    showConfirmButton: false,
    timerProgressBar: true,
});

var oldUniversityId, oldEducationId, oldDepartment, oldActivitiesSocieties, oldStartYear, oldFinishYear;
function getEducationInformationForEdit(id) {
    $.get("/api/education/get", { EducationId: id }).done(function (result) {
        if (result.status === true) {
            oldUniversityId = result.data.universityId; oldActivitiesSocieties = result.data.activitiesSocieties;
            oldDepartment = result.data.department; oldEducationId = result.data.educationId;
            oldStartYear = result.data.startyear; oldFinishYear = result.data.finishyear;
            //gelen verileri old olarak alıyoruz ki edit ederken değişiklik yapılmış mı kontrol edilip ona göre post yollasın diye.
            $('#education-edit-modal').modal('show');
            $('#education-edit-modal-body').load("/education/editview/?ActivitiesSocieties=" + result.data.activitiesSocieties + "&Department=" + result.data.department + "&UniversityId=" + result.data.universityId + "&EducationId=" + result.data.educationId + "&Startyear=" + result.data.startyear + "&Finishyear=" + result.data.finishyear + "&AreUniversityorDepartmentCanEditable=" + result.data.areUniversityorDepartmentCanEditable);
        }
    });
};

var universityId, educationId, department, activitiesSocieties, startYear, finishYear;

$(document).on('submit', '#edit-education-form', function (evt) {
    evt.preventDefault();
    universityId = $("#edit-education-form select[name=UniversityId]").val();
    department = $("#edit-education-form input[name=Department]").val();
    activitiesSocieties = $("#edit-education-form input[name=ActivitiesSocieties]").val();
    startYear = $("#edit-education-form select[name=Startyear]").val();
    finishYear = $("#edit-education-form select[name=Finishyear]").val();
    educationId = $("#edit-education-form input[name=EducationId]").val();

    if (department != null && department.length <= 0) {
        Toast.fire({
            icon: 'warning',
            html: '<span class="font-weight-bold text-black-50 ml-1">Bölümünüzü yazmalısınız</span>'
        });
    }
    else {
        if (universityId != oldUniversityId || department != oldDepartment || educationId != oldEducationId || activitiesSocieties != oldActivitiesSocieties || startYear != oldStartYear || finishYear != oldFinishYear) {
            console.log(universityId + ' ' + oldUniversityId);
            console.log(department + ' ' + oldDepartment);
            console.log(educationId + ' ' + oldEducationId);
            console.log(activitiesSocieties + ' ' + oldActivitiesSocieties);
            console.log(startYear + ' ' + oldStartYear);
            console.log(finishYear + ' ' + oldFinishYear);

            apiEducationPost();
        }
        else {
            Toast.fire({
                icon: 'error',
                html: '<span class="font-weight-bold text-black-50 ml-1">Bir değişiklik yapmadınız</span>'
            });
        }
    }
});

$('#add-education-form').submit(function (evt) {
    evt.preventDefault();
    universityId = $("#add-education-form select[name=UniversityId]").val();
    department = $("#add-education-form input[name=Department]").val();
    activitiesSocieties = $("#add-education-form input[name=ActivitiesSocieties]").val();
    startYear = $("#add-education-form select[name=Startyear]").val();
    finishYear = $("#add-education-form select[name=Finishyear]").val();
    educationId = $("#add-education-form input[name=EducationId]").val();

    if (department != null && department.length <= 0) {
        Toast.fire({
            icon: 'warning',
            html: '<span class="font-weight-bold text-black-50 ml-1">Bölümünüzü yazmalısınız</span>'
        });
    }
    else {
        apiEducationPost();
    }
});

function apiEducationPost() {
    $.post("/api/education/post", { UniversityId: universityId, Department: department, ActivitiesSocieties: activitiesSocieties, Startyear: startYear, Finishyear: finishYear, EducationId: educationId, __RequestVerificationToken: validatetoken })
        .done(function (result) {
            if (result.status === true) {
                $('#edit-education-button').prop('disabled', true);
                $('#add-education-button').prop('disabled', true);
                Toast.fire({
                    icon: 'success',
                    html: '<span class="font-weight-bold text-black-50 ml-1">' + result.message + '</span>'
                });
                //yenilemek yerine bir eğitimlerin olduğu component'i yeniden çağırmam lazım
                setTimeout(function () { location.reload(); }, 2000)
            }
            else {
                Toast.fire({
                    icon: 'warning',
                    html: '<span class="font-weight-bold text-black-50 ml-1">' + result.message + '</span>'
                });
            }
        });
};
var _educationIdForRemove;

function getEducationInformationForRemove(id) {
    $.get("/api/education/get", { EducationId: id }).done(function (result) {
        if (result.status === true) {
            $('#education-remove-modal').modal('show');
            _educationIdForRemove = id;

        }
    });
};


function deleteEducation() {

    $.post("/api/education/post", { educationIdForRemove: _educationIdForRemove, __RequestVerificationToken: validatetoken }).done(function (result) {
        if (result.status === true) {

            Toast.fire({
                icon: 'success',
                html: '<span class="font-weight-bold text-black-50 ml-1">' + result.message + '</span>'
            });

        }
        else if (result.message != null) {

            Toast.fire({
                icon: 'info',
                html: '<span class="font-weight-bold text-black-50 ml-1">' + result.message + '</span>'
            });

        }
    });

    setTimeout(function () { location.reload(); }, 2000)
};