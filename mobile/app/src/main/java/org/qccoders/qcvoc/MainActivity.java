/*
    Copyright (c) QC Coders. All rights reserved. Licensed under the GPLv3 license. See LICENSE file
    in the project root for full license information.
*/

package org.qccoders.qcvoc;

import android.Manifest;
import android.app.Activity;
import android.content.ContentResolver;
import android.content.Intent;
import android.content.pm.PackageManager;
import android.graphics.Bitmap;
import android.net.Uri;
import android.os.Environment;
import android.os.StrictMode;
import android.provider.MediaStore;
import android.support.annotation.NonNull;
import android.support.design.widget.Snackbar;
import android.support.v4.app.ActivityCompat;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Base64;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.webkit.WebResourceRequest;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.ProgressBar;
import android.widget.Toast;

import com.google.android.gms.samples.vision.barcodereader.BarcodeCaptureActivity;
import com.google.android.gms.vision.barcode.Barcode;

import java.io.ByteArrayOutputStream;
import java.io.File;

import static android.view.KeyEvent.KEYCODE_VOLUME_DOWN;
import static android.view.KeyEvent.KEYCODE_VOLUME_UP;

public class MainActivity extends AppCompatActivity {
    private WebView webview;
    private boolean developmentEnvironment = false;
    private ProgressBar progressBar;
    private String callback;
    private Uri mImageUri;

    private static final String prodUrl = "http://qcvoc-prod.s3-website-us-east-1.amazonaws.com";
    private static final String devUrl = "http://qcvoc-dev.s3-website-us-east-1.amazonaws.com";

    private static final int[] environmentCode = {KEYCODE_VOLUME_UP, KEYCODE_VOLUME_UP, KEYCODE_VOLUME_UP,
            KEYCODE_VOLUME_DOWN, KEYCODE_VOLUME_UP, KEYCODE_VOLUME_UP, KEYCODE_VOLUME_DOWN,
            KEYCODE_VOLUME_UP, KEYCODE_VOLUME_UP, KEYCODE_VOLUME_UP, KEYCODE_VOLUME_UP,
            KEYCODE_VOLUME_DOWN};
    private int codeIndex = 0;

    private static final int BARCODE_REQUEST = 42;
    private static final int PHOTO_REQUEST = 1888;
    private static final int PHOTO_PERMISSION_CODE = 100;

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

            @Override
            public boolean shouldOverrideUrlLoading(WebView view, WebResourceRequest request) {
                String currentHost = "http://" + Uri.parse(view.getUrl()).getHost();
                if (!currentHost.equals(prodUrl) && !currentHost.equals(devUrl)) {
                    return true;
                }

                String query = request.getUrl().getQuery();

                if (query != null) {
                    String queryCommand = query.substring(0, query.indexOf('&'));
                    callback = query.substring(query.indexOf('&') + 1);

                    if (queryCommand.equals("scan")) {
                        scanBarcode();
                        return true;
                    } else if (queryCommand.equals("acquirePhoto")) {
                        takePhoto();
                        return true;
                    }
                }

                String requestHost = "http://" + request.getUrl().getHost();
                return !requestHost.equals(prodUrl) && !requestHost.equals(devUrl);
            }
        });

        webview.getSettings().setJavaScriptEnabled(true);
        webview.getSettings().setDomStorageEnabled(true);
        webview.loadUrl(prodUrl);
    }

    public void scanBarcode() {
        Intent intent = new Intent(this, BarcodeCaptureActivity.class);
        intent.putExtra(BarcodeCaptureActivity.AutoFocus, true);

        startActivityForResult(intent, BARCODE_REQUEST);
    }

    public void takePhoto() {
        if (ActivityCompat.checkSelfPermission(this, Manifest.permission.CAMERA) != PackageManager.PERMISSION_GRANTED
                || ActivityCompat.checkSelfPermission(this, Manifest.permission.WRITE_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED){
            ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.CAMERA,
                            Manifest.permission.WRITE_EXTERNAL_STORAGE},
                            PHOTO_PERMISSION_CODE);
        } else {
            captureImage();
        }
    }

    private void captureImage() {
        StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
        StrictMode.setVmPolicy(builder.build());

        Intent cameraIntent = new Intent(android.provider.MediaStore.ACTION_IMAGE_CAPTURE);
        File photo;
        try
        {
            // place where to store camera taken picture
            photo = File.createTempFile("photo", ".jpg", Environment.getExternalStorageDirectory());
            photo.delete();
        }
        catch(Exception e)
        {
            Log.v("MainActivity", "Error creating temporary file to store photo", e);
            Toast.makeText(this, "Error creating temporary file to store photo", Toast.LENGTH_LONG).show();
            return;
        }
        mImageUri = Uri.fromFile(photo);
        cameraIntent.putExtra(MediaStore.EXTRA_OUTPUT, mImageUri);

        startActivityForResult(cameraIntent, PHOTO_REQUEST);
    }

    private Bitmap retrievePhoto()
    {
        ContentResolver cr = this.getContentResolver();
        Bitmap bitmap;
        try
        {
            bitmap = android.provider.MediaStore.Images.Media.getBitmap(cr, mImageUri);
            return transformPhoto(bitmap);
        }
        catch (Exception e)
        {
            Toast.makeText(this, "Error retrieving photo", Toast.LENGTH_LONG).show();
            Log.d("MainActivity", "Error retrieving photo", e);
            return Bitmap.createBitmap(300, 300, Bitmap.Config.ARGB_8888);
        }
    }

    private Bitmap transformPhoto(Bitmap bitmap) {
        // Crop to square
        if (bitmap.getWidth() >= bitmap.getHeight()){
            bitmap = Bitmap.createBitmap(
                    bitmap,
                    bitmap.getWidth()/2 - bitmap.getHeight()/2,
                    0,
                    bitmap.getHeight(),
                    bitmap.getHeight()
            );
        } else {
            bitmap= Bitmap.createBitmap(
                    bitmap,
                    0,
                    bitmap.getHeight()/2 - bitmap.getWidth()/2,
                    bitmap.getWidth(),
                    bitmap.getWidth()
            );
        }
        // Scale down
        return Bitmap.createScaledBitmap(bitmap, 300, 300, false);
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, @NonNull String[] permissions, @NonNull int[] grantResults) {
        super.onRequestPermissionsResult(requestCode, permissions, grantResults);
        if (requestCode == PHOTO_PERMISSION_CODE) {
            for (int grantResult : grantResults) {
                if (grantResult != PackageManager.PERMISSION_GRANTED) {
                    Toast.makeText(this, "Required permissions were not granted for photo taking", Toast.LENGTH_LONG).show();
                    return;
                }
            }
            captureImage();
        }
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == BARCODE_REQUEST) {
            if (data != null) {
                Barcode barcode = data.getParcelableExtra(BarcodeCaptureActivity.BarcodeObject);

                Log.d("MainActivity", barcode.displayValue);

                webview.evaluateJavascript(
                        callback + "(" + barcode.displayValue + ");",
                        null);
            }
        }
        if (requestCode == PHOTO_REQUEST && resultCode == Activity.RESULT_OK) {
            Bitmap bitmap = retrievePhoto();

            ByteArrayOutputStream stream = new ByteArrayOutputStream();
            bitmap.compress(Bitmap.CompressFormat.JPEG, 70, stream);
            byte[] byteArray = stream.toByteArray();
            String photo = "data:image/jpeg;base64," + Base64.encodeToString(byteArray, Base64.NO_WRAP);

            Log.d("MainActivity", "Photo of size " + photo.length() + " bytes taken");
            webview.evaluateJavascript(
                    callback + "(" + photo + ")",
                    null
            );
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

            String newUrl = developmentEnvironment ? devUrl : prodUrl;

            webview.loadUrl(newUrl);
        }

        return super.onKeyUp(keycode, event);
    }
}
