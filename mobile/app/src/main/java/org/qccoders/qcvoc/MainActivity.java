/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

package org.qccoders.qcvoc;

import android.content.Intent;
import android.graphics.Bitmap;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.webkit.JavascriptInterface;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.ProgressBar;

import com.google.android.gms.samples.vision.barcodereader.BarcodeCaptureActivity;
import com.google.android.gms.vision.barcode.Barcode;

import static android.view.KeyEvent.KEYCODE_VOLUME_DOWN;
import static android.view.KeyEvent.KEYCODE_VOLUME_UP;

public class MainActivity extends AppCompatActivity {
    private WebView webview;
    private boolean developmentEnvironment = false;
    private ProgressBar progressBar;
    private String callback;

    private final int[] environmentCode = {KEYCODE_VOLUME_UP, KEYCODE_VOLUME_UP, KEYCODE_VOLUME_UP,
            KEYCODE_VOLUME_DOWN, KEYCODE_VOLUME_UP, KEYCODE_VOLUME_UP, KEYCODE_VOLUME_DOWN,
            KEYCODE_VOLUME_UP, KEYCODE_VOLUME_UP, KEYCODE_VOLUME_UP, KEYCODE_VOLUME_UP,
            KEYCODE_VOLUME_DOWN};
    private int codeIndex = 0;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        progressBar = findViewById(R.id.loading_spinner);
        progressBar.setVisibility(View.GONE);

        webview = findViewById(R.id.webView);
        webview.setWebViewClient(new WebViewClient() {
            @Override
            public void onPageStarted(WebView view, String url, Bitmap favicon) {
                super.onPageStarted(view, url, favicon);
                progressBar.setVisibility(View.VISIBLE);
            }

            @Override
            public void onPageFinished(WebView view, String url) {
                super.onPageFinished(view, url);
                progressBar.setVisibility(View.GONE);
            }
        });

        webview.addJavascriptInterface(this, "Android");
        webview.getSettings().setJavaScriptEnabled(true);
        webview.getSettings().setDomStorageEnabled(true);
        webview.loadUrl("http://qcvoc-prod.s3-website-us-east-1.amazonaws.com");
    }

    @JavascriptInterface
    public void scanBarcode(String callback) {
        this.callback = callback;

        Intent intent = new Intent(this, BarcodeCaptureActivity.class);
        intent.putExtra(BarcodeCaptureActivity.AutoFocus, true);

        startActivityForResult(intent, 42);
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (data != null) {
            Barcode barcode = data.getParcelableExtra(BarcodeCaptureActivity.BarcodeObject);

            Log.d("MainActivity", barcode.displayValue);

            webview.evaluateJavascript(
                    callback + "(" + barcode.displayValue + ");",
                    null);
        }
    }

    @Override
    public boolean onKeyUp(int keycode, KeyEvent event) {

        if (event.getKeyCode() == environmentCode[codeIndex]) {
            codeIndex += 1;
        } else {
            codeIndex = 0;
        }

        if (codeIndex == environmentCode.length) {
            codeIndex = 0;
            developmentEnvironment = !developmentEnvironment;

            String message = "Environment switched to " +
                    (developmentEnvironment ? "development" : "production");

            Log.d("MainActivity", message);

            Snackbar.make(
                    findViewById(R.id.mainLayout),
                    message,
                    Snackbar.LENGTH_SHORT)
                    .show();

            String newUrl = developmentEnvironment
                    ? "http://qcvoc-dev.s3-website-us-east-1.amazonaws.com"
                    : "http://qcvoc-prod.s3-website-us-east-1.amazonaws.com";

            webview.loadUrl(newUrl);
        }

        return super.onKeyUp(keycode, event);
    }
}