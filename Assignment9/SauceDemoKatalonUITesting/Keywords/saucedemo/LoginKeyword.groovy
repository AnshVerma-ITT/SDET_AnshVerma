package saucedemo

import static com.kms.katalon.core.testobject.ObjectRepository.findTestObject

import com.kms.katalon.core.annotation.Keyword
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI

class LoginKeyword {

    @Keyword
    def login(String username, String password) {

        WebUI.waitForElementVisible(
            findTestObject('Object Repository/Login/input_Username'),
            10
        )

        WebUI.click(
            findTestObject('Object Repository/Login/input_Username')
        )

        WebUI.setText(
            findTestObject('Object Repository/Login/input_Username'),
            username
        )

        WebUI.click(
            findTestObject('Object Repository/Login/input_Password')
        )

        WebUI.setText(
            findTestObject('Object Repository/Login/input_Password'),
            password
        )

        WebUI.click(
            findTestObject('Object Repository/Login/input_login-button')
        )

        WebUI.delay(2)
    }
}