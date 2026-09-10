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

WebUI.setText(findTestObject('Login/input_Username'), 'standard_user')

WebUI.setEncryptedText(findTestObject('Login/input_Password'), 'qcu24s4901FyWDTwXGr6XA==')

WebUI.sendKeys(findTestObject('Login/input_Password'), Keys.chord(Keys.ENTER))

WebUI.click(findTestObject('Inventory/button_add-to-cart-sauce-labs-backpack'))

WebUI.click(findTestObject('Inventory/button_add-to-cart-sauce-labs-bike-light'))

WebUI.click(findTestObject('Common/a_2'))

WebUI.rightClick(findTestObject('Cart/button_checkout'))

WebUI.assertElementText(findTestObject('Cart/button_checkout'), 'Checkout', 0)

WebUI.click(findTestObject('Cart/button_checkout'))

WebUI.setText(findTestObject('Checkout/input_First Name'), FirstName)

WebUI.setText(findTestObject('Checkout/input_Last Name'), LastName)

WebUI.setText(findTestObject('Checkout/input_Zip_Postal Code'), PostalCode)

WebUI.click(findTestObject('Checkout/input_continue'))

WebUI.rightClick(findTestObject('Checkout/button_finish'))

WebUI.assertElementText(findTestObject('Checkout/button_finish'), 'Finish', 0)

WebUI.click(findTestObject('Checkout/button_finish'))

WebUI.rightClick(findTestObject('Checkout/h2_Thank you for your order'))

WebUI.assertElementText(findTestObject('Checkout/h2_Thank you for your order'), 'Thank you for your order!', 0)

WebUI.verifyCheckpoint(findCheckpoint('CheckPoints/CP_CheckoutTestData'), false)

