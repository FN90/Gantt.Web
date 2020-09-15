(function () {
    // add month scale
    gantt.config.scale_unit = "week";
    gantt.config.duration_unit = "minute";//an hour
    gantt.config.duration_step = 1;

    gantt.config.step = 1;
    gantt.config.drag_move = false; //disables the possibility to move tasks by dnd
    gantt.config.drag_links = false; //disables the possibility to create links by dnd
    gantt.config.drag_progress = false; //disables the possibility to change the task //progress by dragging the progress knob
    gantt.config.drag_resize = false; //disables the possibility to resize tasks by dnd
    gantt.templates.date_scale = function (date) {
        var dateToStr = gantt.date.date_to_str("%d %M");
        var endDate = gantt.date.add(gantt.date.add(date, 1, "week"), -1, "day");
        return dateToStr(date) + " - " + dateToStr(endDate);
    };


    gantt.config.subscales = [
        { unit: "day", step: 1, date: "%D" },
        { unit: "hour", step: 8, date: "%H:00" },
        { unit: "minute", step: 15, date: "%i" }
    ];
    //gantt.config.scales = [
    //    { unit: "hour", step: 1, format: "%g %a" },
    //    { unit: "day", step: 1, format: "%j %F, %l" },
    //    { unit: "minute", step: 15, format: "%i" }
    //];
    gantt.config.scale_height = 50;

    // configure milestone description
    gantt.templates.rightside_text = function (start, end, task) {
        if (task.type == gantt.config.types.milestone) {
            return task.text;
        }
        return "";
    };

    gantt.templates.tooltip_text = function (start, end, task) {
        var tooltipDescription = "";
        var dateToStr = gantt.date.date_to_str("%d %M %Y %H:%i");

        jQuery.ajax({
            url: "/api/GetTask/" + task.id,
            async: false,
            success: function (data) {
                tooltipDescription = "<b>Tache:</b> " + task.text +
                    "<br/><b>Date départ:</b> " + dateToStr(task.start_date) +
                    "<br/><b>Date de fin:</b> " + dateToStr(task.end_date) +
                    "<br/><b>Centre de charge:</b> " + data.CentreCharge +
                    "<br/><b>Employé:</b> " + data.Employe +
                    "<br/><b>Durée:</b> " + task.duration + " minute(s)";
            }
        });
        return tooltipDescription;

    };

    // add section to type selection: task, project or milestone
    gantt.config.lightbox.sections = [
        { name: "description", height: 70, map_to: "text", type: "textarea", focus: true },
        { name: "type", type: "typeselect", map_to: "type" },
        { name: "time", height: 72, type: "duration", map_to: "auto" }
    ];

    gantt.config.xml_date = "%Y-%m-%d %H:%i:%s"; // format of dates in XML

    gantt.locale = { date: { month_full: ["Janvier", "Février", "Mars", "Avril", "Mai", "Juin", "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre"], month_short: ["Jan", "Fév", "Mar", "Avr", "Mai", "Juin", "Juil", "Aôu", "Sep", "Oct", "Nov", "Déc"], day_full: ["Dimanche", "Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi", "Samedi"], day_short: ["Dim", "Lun", "Mar", "Mer", "Jeu", "Ven", "Sam"] }, labels: { new_task: "Tâche neuve", icon_save: "Enregistrer", icon_cancel: "Annuler", icon_details: "Détails", icon_edit: "Modifier", icon_delete: "Effacer", confirm_closing: "", confirm_deleting: "L'événement sera effacé sans appel, êtes-vous sûr ?", section_description: "Description", section_time: "Période", section_type: "Type", column_text: "Tâche neuve", column_start_date: "Date initiale", column_duration: "Durée", column_add: "", confirm_link_deleting: "seront supprimées", link_start: "(début)", link_end: "(fin)", type_task: "Task", type_project: "Project", type_milestone: "Milestone", minutes: "Minutes", hours: "Heures", days: "Jours", weeks: "Semaine", months: "Mois", years: "Années" } };

    gantt.init("ganttContainer"); // initialize gantt
    console.log('projet id: ' + projetId);
    if (projetId != 0) {
        gantt.load("/Api/Data/" + projetId, "json");
    }
    else {
        gantt.load("/Api/Data", "json");
    }

    // enable dataProcessor
    var dp = new dataProcessor("/Home/Save");
    dp.init(gantt);



    gantt.attachEvent("onBeforeLightbox", function (id) {
        if (!isAdmin) {
            $('#youAreNotAuthorizedModal').modal('toggle');
        }
        else {
            var task = gantt.getTask(id);
            console.log(task);

            if (task.parent == 0) {

                $('#addProject').modal('toggle');
            }
            else {

                //task.my_template = "<span id='title1'>Holders: </span>" + task.users
                //    + "<span id='title2'>Progress: </span>" + task.progress * 100 + " %";
                jQuery.get("/api/TaskType/" + task.parent, function (data) {
                    console.log('Task type: ', data);
                    if (data.type === 'composant') {
                        addNewTask(task);
                    }
                    else if (data.type === 'task') {
                        $('#youCannotAddTaskModal').modal('toggle');
                    }
                    else if (data.type === 'project') {
                        addNewComposant(task);
                    }
                });
            }
        }


        return false;
    });


    function addNewComposant(project) {

        // get current task parent
        jQuery.get("/api/GetComposantTasks/" + project.parent, function (data) {
            $('#ComposantParentId').val(data.task.Id);
            $('.projectName').html(data.task.Text);
            $('#addComposant').modal('toggle');
            $('#datetimepicker2').datetimepicker({
                //minDate: data.task.StartDate,
                //format: 'L'
            });
        });
    }

    function addNewTask(task) {
        // get current task parent
        jQuery.get("/api/GetComposantTasks/" + task.parent, function (data) {
            console.log(data);
            // date format options
            var options = { year: 'numeric', month: 'numeric', day: 'numeric', minute: 'numeric', hour: 'numeric' };
            // change dialog title, put parent task text and id
            $('.composantText').html(data.task.Text);
            $('#ParentId').val(data.task.Id);

            // init datetimepicker
            $('#datetimepicker1').datetimepicker({
                //minDate: data.task.StartDate
            });

            // if the current parent has tasks, get them to populate precedent task options
            if (data.tasks.length > 0) {
                $('#StartDate').prop('readonly', true);
                // hide no-task warning
                $('#firstTaskWarning').hide();
                // show the list of options if it is hidden
                $('#PredecessorId').show();
                // remove all options from PredecessorId
                $('#PredecessorId').find('option').remove().end().append('<option value="">Sélectionner...</option>').val('');
                for (i = 0; i < data.tasks.length; i++) {
                    //$('#PredecessorId').append(new Option(data.tasks[i].text, data.tasks[i].id));
                    // format the date
                    var date = data.tasks[i].StartDate;
                    console.log('date : ' + date)
                    var dateMillis = parseInt(date.replace("/Date(", "").replace(")/", ""));
                    //dateMillis = dateMillis - (3 * 60 * 60 * 1000); // fix time: date - 3 hours
                    date = new Date(parseInt(dateMillis, 10));
                    // add options to the precedent task select
                    $('#PredecessorId').append('<option value="' + data.tasks[i].Id + '" data-date="' + date + '" data-duration="' + data.tasks[i].Duration + '">' + data.tasks[i].Text + '</option>');
                }


                $('#PredecessorId').on('change', function () {
                    var startDate = new Date($(this).children('option:selected').data('date'));
                    var duration = parseInt($(this).children('option:selected').data('duration'));
                    var newStartDate = new Date();
                    // set the value of the hidden input to be sent to the backend
                    $('#PredecessorValueId').val($(this).val());
                    newStartDate = startDate.addMinutes(duration);
                    console.log('startDate: ' + $(this).children('option:selected').data('date'));
                    $('#StartDate').val(newStartDate.toLocaleDateString("en-US", options));
                });

                Date.prototype.addMinutes = function (minutes) {
                    var date = new Date(this.valueOf());
                    date.setMinutes(date.getMinutes() + minutes);
                    return date;
                }
            } else {
                $('#firstTaskWarning').show();
                $('#PredecessorId').hide();
                $('#StartDate').prop('readonly', false);
            }


            $('#taskModal').modal('toggle');
        });
    }

    $('#cancelBtn').on('click', function () {
        //gantt.clearAll(); 
        //gantt.load("/Api/Data", "json");
        //$('#taskModal').modal('toggle');
        location.reload();
    })

    $('#taskModal').on('hidden.bs.modal', function () {
        //gantt.clearAll();
        //gantt.load("/Api/Data", "json");
        location.reload();
    })

    $('#youCannotAddTaskModal').on('hidden.bs.modal', function () {
        location.reload();
    })
    $('#youCannotAddTaskOkBtn').on('click', function () {
        location.reload();
    })


})();