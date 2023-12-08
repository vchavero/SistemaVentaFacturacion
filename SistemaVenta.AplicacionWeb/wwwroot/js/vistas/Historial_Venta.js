﻿
const VISTA_BUSQUEDA = {

    busquedaFecha: () => {

        $("#txtFechaInicio").val("")
        $("#txtFechaFin").val("")
        $("#txtNumeroVenta").val("")

        $(".busqueda-fecha").show()
        $(".busqueda-venta").hide()
    }, busquedaVenta: () => {

        $("#txtFechaInicio").val("")
        $("#txtFechaFin").val("")
        $("#txtNumeroVenta").val("")

        $(".busqueda-fecha").hide()
        $(".busqueda-venta").show()
    }
}

$(document).ready(function () {
    VISTA_BUSQUEDA["busquedaFecha"]()

    $.datepicker.setDefaults($.datepicker.regional["es"])

    $("#txtFechaInicio").datepicker({dateFormat : "dd/mm/yy"})
    $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })

})

$("#cboBuscarPor").change(function () {

    if ($("#cboBuscarPor").val() == "fecha") {
        VISTA_BUSQUEDA["busquedaFecha"]()
    } else {
        VISTA_BUSQUEDA["busquedaVenta"]()
    }

})


$("#btnBuscar").click(function () {

    if ($("#cboBuscarPor").val() == "fecha") {

        if ($("#txtFechaInicio").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {
            toastr.warning("", "Debe ingresar fecha inicio y fin")
            return;
        }
    } else {

        if ($("#txtNumeroVenta").val().trim() == "") {
            toastr.warning("", "Debe ingresar el numero de venta")
            return;
        }
    }

    let numeroVenta = $("#txtNumeroVenta").val()
    let fechaInicio = $("#txtFechaInicio").val()
    let fechaFin = $("#txtFechaFin").val()


    $(".card-body").find("div.row").LoadingOverlay("show");

    fetch(`/Venta/Historial?numeroVenta=${numeroVenta}&fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`)
        .then(response => {
            $(".card-body").find("div.row").LoadingOverlay("hide");
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {

            $("#tbventa tbody").html("");

            if (responseJson.length > 0) {

                responseJson.forEach((venta) => {
                    console.log(venta)
                    $("#tbventa tbody").append(
                        $("<tr>").append(
                            $("<td>").text(venta.fechaRegistro),
                            $("<td>").text(venta.numeroVenta),
                            $("<td>").text(venta.tipoDocumentoVenta),
                            $("<td>").text(venta.documentoCliente),
                            $("<td>").text(venta.nombreCliente),
                            $("<td>").text(venta.total),
                            $("<td>").append(
                                $("<button>").addClass("btn btn-info btn-sm").append(
                                    $("<i>").addClass("fas fa-file-pdf")
                                ).data("venta", venta),
                                $("<button>").addClass((venta.idTipoDocumentoVenta == "1") ? "d-none" :"btn btn-warning btn-xml btn-sm").append(
                                    $("<i>").addClass("fas fa-file")
                                ).data("venta", venta)
                            )
                        )
                    )

                })

            }

        })

})

$("#tbventa tbody").on("click", ".btn-xml", function () {

    let d = $(this).data("venta")
    window.location.href = "/facturas/factura_" + d.idVenta + ".xml";

})

$("#tbventa tbody").on("click", ".btn-info", function () {

    let d = $(this).data("venta")
    console.log(d)
    $("#txtFechaRegistro").val(d.fechaRegistro)
    $("#txtNumVenta").val(d.numeroVenta)
    $("#txtUsuarioRegistro").val(d.usuario)
    $("#txtTipoDocumento").val(d.tipoDocumentoVenta)
    $("#txtDocumentoCliente").val(d.documentoCliente)
    $("#txtNombreCliente").val(d.nombreCliente)
    $("#txtRFC").val(d.rfc)
    $("#txtRegimen").val(d.regimen)
    $("#txtCodigoPostal").val(d.codigoPostal)
    $("#txtUsoCFDI").val(d.usoCFDI)
    $("#txtFormaPago").val(d.formaPago)
    $("#txtMetodoPago").val(d.metodoPago)
    $("#txtSubTotal").val(d.subTotal)
    $("#txtIGV").val(d.impuestoTotal)
    $("#txtTotal").val(d.total)


    $("#tbProductos tbody").html("");

    d.detalleVenta.forEach((item) => {

        $("#tbProductos tbody").append(
            $("<tr>").append(
                $("<td>").text(item.descripcionProducto),
                $("<td>").text(item.cantidad),
                $("<td>").text(item.precio),
                $("<td>").text(item.total)
            )
        )

    })

    $("#linkImprimir").attr("href",`/Venta/MostrarPDFVenta?numeroVenta=${d.numeroVenta}`)

    $("#modalData").modal("show");

})