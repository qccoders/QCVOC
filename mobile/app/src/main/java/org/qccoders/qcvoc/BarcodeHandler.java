package org.qccoders.qcvoc;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;

import com.google.android.gms.samples.vision.barcodereader.BarcodeCaptureActivity;
import com.google.android.gms.vision.barcode.Barcode;

import static org.qccoders.qcvoc.Constants.BARCODE_REQUEST;

class BarcodeHandler {
    private Context mContext;
    private String callback;

    public BarcodeHandler(Context mContext) {
        this.mContext = mContext;
    }

    public void scanBarcode(String callback) {
        this.callback = callback;

        Intent intent = new Intent(mContext, BarcodeCaptureActivity.class);
        intent.putExtra(BarcodeCaptureActivity.AutoFocus, true);

        ((Activity) mContext).startActivityForResult(intent, BARCODE_REQUEST);
    }

    public String returnBarcode(Intent data) {
        if (data != null) {
            Barcode barcode = data.getParcelableExtra(BarcodeCaptureActivity.BarcodeObject);

            return callback + "('" + barcode.displayValue + "')";
        }

        return null;
    }
}
