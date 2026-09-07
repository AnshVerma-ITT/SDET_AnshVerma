import static com.kms.katalon.core.checkpoint.CheckpointFactory.findCheckpoint
import static com.kms.katalon.core.testcase.TestCaseFactory.findTestCase
import static com.kms.katalon.core.testdata.TestDataFactory.findTestData
import static com.kms.katalon.core.testobject.ObjectRepository.findTestObject
import static com.kms.katalon.core.testobject.ObjectRepository.findWindowsObject
import com.kms.katalon.core.checkpoint.Checkpoint as Checkpoint
import com.kms.katalon.core.cucumber.keyword.CucumberBuiltinKeywords as CucumberKW
import com.kms.katalon.core.llm.keyword.LlmKeywords as LLM
import com.kms.katalon.core.mobile.keyword.MobileBuiltInKeywords as Mobile
import com.kms.katalon.core.model.FailureHandling as FailureHandling
import com.kms.katalon.core.testcase.TestCase as TestCase
import com.kms.katalon.core.testdata.TestData as TestData
import com.kms.katalon.core.testng.keyword.TestNGBuiltinKeywords as TestNGKW
import com.kms.katalon.core.testobject.TestObject as TestObject
import com.kms.katalon.core.webservice.keyword.WSBuiltInKeywords as WS
import com.kms.katalon.core.webui.keyword.WebUiBuiltInKeywords as WebUI
import com.kms.katalon.core.windows.keyword.WindowsBuiltinKeywords as Windows
import internal.GlobalVariable as GlobalVariable
import org.openqa.selenium.Keys as Keys

WebUI.openBrowser(null)

WebUI.navigateToUrl('https://www.saucedemo.com/')

WebUI.click(findTestObject('Page_Swag Labs/input_Username'))

WebUI.rightClick(findTestObject('Page_Swag Labs/input_Username'))

WebUI.rightClick(findTestObject('Page_Swag Labs/input_Username'))

WebUI.assertElementPresent(findTestObject('Page_Swag Labs/input_Username'), 0)

WebUI.assertElementPresent(findTestObject('Page_Swag Labs/input_Username'), 0)

WebUI.setText(findTestObject('Page_Swag Labs/input_Username'), 'standard_user')

WebUI.click(findTestObject('Page_Swag Labs/input_Password'))

WebUI.rightClick(findTestObject('Page_Swag Labs/input_Password'))

WebUI.rightClick(findTestObject('Page_Swag Labs/input_Password'))

WebUI.assertElementPresent(findTestObject('Page_Swag Labs/input_Password'), 0)

WebUI.assertElementPresent(findTestObject('Page_Swag Labs/input_Password'), 0)

WebUI.setEncryptedText(findTestObject('Page_Swag Labs/input_Password'), 'qcu24s4901FyWDTwXGr6XA==')

WebUI.rightClick(findTestObject('Page_Swag Labs/input_login-button'))

WebUI.rightClick(findTestObject('Page_Swag Labs/input_login-button'))

WebUI.assertElementClickable(findTestObject('Page_Swag Labs/input_login-button'), 0)

WebUI.assertElementClickable(findTestObject('Page_Swag Labs/input_login-button'), 0)

WebUI.click(findTestObject('Page_Swag Labs/input_login-button'))

WebUI.rightClick(findTestObject('Page_Swag Labs/span_Products'))

WebUI.rightClick(findTestObject('Page_Swag Labs/span_Products'))

WebUI.assertElementText(findTestObject('Page_Swag Labs/span_Products'), 'Products', 0)

WebUI.assertElementText(findTestObject('Page_Swag Labs/span_Products'), 'Products', 0)

