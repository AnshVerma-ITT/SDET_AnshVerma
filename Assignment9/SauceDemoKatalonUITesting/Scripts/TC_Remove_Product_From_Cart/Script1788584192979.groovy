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

WebUI.setText(findTestObject('Page_Swag Labs/input_Username'), 'standard_user')

WebUI.setEncryptedText(findTestObject('Page_Swag Labs/input_Password'), 'qcu24s4901FyWDTwXGr6XA==')

WebUI.click(findTestObject('Page_Swag Labs/input_login-button'))

WebUI.click(findTestObject('Page_Swag Labs/button_add-to-cart-sauce-labs-backpack'))

WebUI.click(findTestObject('Page_Swag Labs/a_1'))

WebUI.rightClick(findTestObject('Page_Swag Labs/button_remove-sauce-labs-backpack'))

WebUI.assertElementText(findTestObject('Page_Swag Labs/button_remove-sauce-labs-backpack'), 'Remove', 0)

WebUI.rightClick(findTestObject('Page_Swag Labs/div_1'))

WebUI.assertElementText(findTestObject('Page_Swag Labs/div_1'), '1', 0)

WebUI.click(findTestObject('Page_Swag Labs/button_remove-sauce-labs-backpack'))

WebUI.rightClick(findTestObject('Page_Swag Labs/a_1'))

WebUI.assertElementText(findTestObject('Page_Swag Labs/a_1'), '1', 0)

