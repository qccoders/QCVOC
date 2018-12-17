package org.qccoders.qcvoc.features;

import android.app.Activity;
import android.content.ContentResolver;
import android.content.Context;
import android.content.Intent;
import android.graphics.Bitmap;
import android.graphics.Matrix;
import android.net.Uri;
import android.os.Environment;
import android.provider.MediaStore;
import android.support.v4.content.FileProvider;
import android.util.Base64;
import android.util.Log;
import android.widget.Toast;

import java.io.ByteArrayOutputStream;
import java.io.File;

import static org.qccoders.qcvoc.shared.Constants.PHOTO_REQUEST;

public class PhotoHandler {
    private Context mContext;
    private Uri mPhotoUri;
    private String callback;

    public PhotoHandler(Context mContext) {
        this.mContext = mContext;
    }

    public void setCallback(String callback) {
        this.callback = callback;
    }

    public void capturePhotoWithCallback(String callback) {
        setCallback(callback);
        capturePhoto();
    }

    public void capturePhoto() {
        Intent cameraIntent = new Intent(android.provider.MediaStore.ACTION_IMAGE_CAPTURE);
        File photo;
        try
        {
            // place where to store camera taken picture
            File photoDir = mContext.getExternalFilesDir(Environment.DIRECTORY_PICTURES);
            photo = File.createTempFile("photo", ".jpg", photoDir);
            photo.delete();
        }
        catch(Exception e)
        {
            Log.d("PhotoHandler", "Error creating temporary file to store photo", e);
            Toast.makeText(mContext, "Error creating temporary file to store photo", Toast.LENGTH_LONG).show();
            return;
        }

        mPhotoUri = FileProvider.getUriForFile(mContext,
                "org.qccoders.qcvoc.fileprovider",
                photo);
        cameraIntent.putExtra(MediaStore.EXTRA_OUTPUT, mPhotoUri);

        ((Activity) mContext).startActivityForResult(cameraIntent, PHOTO_REQUEST);
    }

    public String returnPhoto() {
        Bitmap bitmap = retrievePhoto();

        ByteArrayOutputStream stream = new ByteArrayOutputStream();
        bitmap.compress(Bitmap.CompressFormat.JPEG, 70, stream);
        byte[] byteArray = stream.toByteArray();
        String photo = "data:image/jpeg;base64," + Base64.encodeToString(byteArray, Base64.NO_WRAP);

        Log.d("MainActivity", "Photo of size " + photo.length() + " bytes taken");
        return callback + "('" + photo + "')";

    }

    private Bitmap retrievePhoto() {
        ContentResolver cr = mContext.getContentResolver();
        Bitmap bitmap;
        try
        {
            bitmap = android.provider.MediaStore.Images.Media.getBitmap(cr, mPhotoUri);
            return transformPhoto(bitmap);
        }
        catch (Exception e)
        {
            Toast.makeText(mContext, "Error retrieving photo", Toast.LENGTH_LONG).show();
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
        bitmap = Bitmap.createScaledBitmap(bitmap, 300, 300, false);
        // Rotate
        Matrix matrix = new Matrix();
        matrix.postRotate(90);
        return Bitmap.createBitmap(bitmap, 0, 0, bitmap.getWidth(), bitmap.getHeight(), matrix, true);
    }
}
