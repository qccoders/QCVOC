package org.qccoders.qcvoc.security

import java.util.*

sealed class User

data class UnauthenticatedUser(
        val username: String,
        val password: String): User()

data class AuthenticatedUser(
        val id: String,
        val username: String,
        val token: String): User()

typealias ErrorMessage = String

sealed class Result<out F, out S>
data class Success<S>(val t: S): Result<Nothing, S>()
data class Failure<F>(val f: F): Result<F, Nothing>()

fun login(username: String, password: String): Result<String, User> {

    // Check with auth server later.
    val isAuthenticated = true

    if(isAuthenticated) {
        val guid = "This should be a guid from the server."
        val username = "This should be the username from server"
        val token = "This should be the JWT from server"
        return Success(AuthenticatedUser(guid, username, token))
    }
    else
        return Failure("Incorrect credentials. Please try again.")
}

fun isEmailValid(email: String): Boolean {
    //TODO: Replace this with your own logic
    return email.contains("@")
}

fun isPasswordValid(password: String): Boolean {
    //TODO: Replace this with your own logic
    return password.length > 4
}

