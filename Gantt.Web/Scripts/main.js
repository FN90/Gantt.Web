﻿!function () { gantt.config.scale_unit = "week", gantt.config.duration_unit = "minute", gantt.config.duration_step = 1, gantt.config.step = 1, gantt.config.drag_move = !1, gantt.config.drag_links = !1, gantt.config.drag_progress = !1, gantt.config.drag_resize = !1, gantt.templates.date_scale = function (t) { var e = gantt.date.date_to_str("%d %M"), a = gantt.date.add(gantt.date.add(t, 1, "week"), -1, "day"); return e(t) + " - " + e(a) }, gantt.config.subscales = [{ unit: "day", step: 1, date: "%D" }, { unit: "hour", step: 8, date: "%H:00" }, { unit: "minute", step: 15, date: "%i" }], gantt.config.scale_height = 50, gantt.templates.rightside_text = function (t, e, a) { return a.type == gantt.config.types.milestone ? a.text : "" }, gantt.templates.tooltip_text = function (t, e, a) { var n = "", o = gantt.date.date_to_str("%d %M %Y %H:%i"); return jQuery.ajax({ url: "/api/GetTask/" + a.id, async: !1, success: function (t) { n = "<b>Tache:</b> " + a.text + "<br/><b>Date départ:</b> " + o(a.start_date) + "<br/><b>Date de fin:</b> " + o(a.end_date) + "<br/><b>Centre de charge:</b> " + t.CentreCharge + "<br/><b>Employé:</b> " + t.Employe + "<br/><b>Durée:</b> " + a.duration + " minute(s)" } }), n }, gantt.config.lightbox.sections = [{ name: "description", height: 70, map_to: "text", type: "textarea", focus: !0 }, { name: "type", type: "typeselect", map_to: "type" }, { name: "time", height: 72, type: "duration", map_to: "auto" }], gantt.config.xml_date = "%Y-%m-%d %H:%i:%s", gantt.locale = { date: { month_full: ["Janvier", "Février", "Mars", "Avril", "Mai", "Juin", "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre"], month_short: ["Jan", "Fév", "Mar", "Avr", "Mai", "Juin", "Juil", "Aôu", "Sep", "Oct", "Nov", "Déc"], day_full: ["Dimanche", "Lundi", "Mardi", "Mercredi", "Jeudi", "Vendredi", "Samedi"], day_short: ["Dim", "Lun", "Mar", "Mer", "Jeu", "Ven", "Sam"] }, labels: { new_task: "Tâche neuve", icon_save: "Enregistrer", icon_cancel: "Annuler", icon_details: "Détails", icon_edit: "Modifier", icon_delete: "Effacer", confirm_closing: "", confirm_deleting: "L'événement sera effacé sans appel, êtes-vous sûr ?", section_description: "Description", section_time: "Période", section_type: "Type", column_text: "Tâche neuve", column_start_date: "Date initiale", column_duration: "Durée", column_add: "", confirm_link_deleting: "seront supprimées", link_start: "(début)", link_end: "(fin)", type_task: "Task", type_project: "Project", type_milestone: "Milestone", minutes: "Minutes", hours: "Heures", days: "Jours", weeks: "Semaine", months: "Mois", years: "Années" } }, gantt.init("ganttContainer"), console.log("projet id: " + projetId), 0 != projetId ? gantt.load("/Api/Data/" + projetId, "json") : gantt.load("/Api/Data", "json"), new dataProcessor("/Home/Save").init(gantt), gantt.attachEvent("onBeforeLightbox", function (t) { if (isAdmin) { var e = gantt.getTask(t); console.log(e), 0 == e.parent ? $("#addProject").modal("toggle") : jQuery.get("/api/TaskType/" + e.parent, function (t) { var a; console.log("Task type: ", t), "composant" === t.type ? function (t) { jQuery.get("/api/GetComposantTasks/" + t.parent, function (t) { console.log(t); var e = { year: "numeric", month: "numeric", day: "numeric", minute: "numeric", hour: "numeric" }; if ($(".composantText").html(t.task.Text), $("#ParentId").val(t.task.Id), $("#datetimepicker1").datetimepicker({}), t.tasks.length > 0) { for ($("#StartDate").prop("readonly", !0), $("#firstTaskWarning").hide(), $("#PredecessorId").show(), $("#PredecessorId").find("option").remove().end().append('<option value="">Sélectionner...</option>').val(""), i = 0; i < t.tasks.length; i++) { var a = t.tasks[i].StartDate; console.log("date : " + a); var n = parseInt(a.replace("/Date(", "").replace(")/", "")); n -= 108e5, a = new Date(parseInt(n, 10)), $("#PredecessorId").append('<option value="' + t.tasks[i].Id + '" data-date="' + a + '" data-duration="' + t.tasks[i].Duration + '">' + t.tasks[i].Text + "</option>") } $("#PredecessorId").on("change", function () { var t = new Date($(this).children("option:selected").data("date")), a = parseInt($(this).children("option:selected").data("duration")), n = new Date; $("#PredecessorValueId").val($(this).val()), n = t.addMinutes(a), console.log("startDate: " + $(this).children("option:selected").data("date")), $("#StartDate").val(n.toLocaleDateString("en-US", e)) }), Date.prototype.addMinutes = function (t) { var e = new Date(this.valueOf()); return e.setMinutes(e.getMinutes() + t), e } } else $("#firstTaskWarning").show(), $("#PredecessorId").hide(), $("#StartDate").prop("readonly", !1); $("#taskModal").modal("toggle") }) }(e) : "task" === t.type ? $("#youCannotAddTaskModal").modal("toggle") : "project" === t.type && (a = e, jQuery.get("/api/GetComposantTasks/" + a.parent, function (t) { $("#ComposantParentId").val(t.task.Id), $(".projectName").html(t.task.Text), $("#addComposant").modal("toggle"), $("#datetimepicker2").datetimepicker({}) })) }) } else $("#youAreNotAuthorizedModal").modal("toggle"); return !1 }), $("#cancelBtn").on("click", function () { location.reload() }), $("#taskModal").on("hidden.bs.modal", function () { location.reload() }), $("#youCannotAddTaskModal").on("hidden.bs.modal", function () { location.reload() }), $("#youCannotAddTaskOkBtn").on("click", function () { location.reload() }) }();