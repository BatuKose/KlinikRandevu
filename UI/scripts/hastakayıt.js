document.getElementById("kInsert").addEventListener("click", async () => {
    try
    {
    const kName = document.getElementById("kName").value.trim();
    const kSurname = document.getElementById("kSurname").value.trim();
    const kTC = document.getElementById("kTC").value.trim();
    const kAddress = document.getElementById("kAddress").value.trim();
    const kPhone = document.getElementById("kPhone").value.trim();
    const kGender = parseInt(document.getElementById("kGender").value);
    const kBloodType = parseInt(document.getElementById("kBloodType").value);
    const kBirthDate = document.getElementById("kBirthDate").value;
    const today = new Date();
    const birthDate = new Date(kBirthDate);
    if (!kName) { alert("Hasta adı zorunludur"); return; }
    if (!kSurname) { alert("Hasta soyadı zorunludur"); return; }
    if (!kTC) { alert("TC Kimlik No zorunludur"); return; }
    if (kTC.length !== 11) { alert("TC Kimlik No 11 haneli olmalıdır"); return; }
    if (!/^\d+$/.test(kTC)) { alert("TC Kimlik No sadece rakamlardan oluşmalıdır"); return; }
    if (kTC[0] === "0") { alert("TC Kimlik No 0 ile başlayamaz"); return; }
    if (!kPhone) { alert("Telefon numarası zorunludur"); return; }
    if (!/^\d{10,11}$/.test(kPhone)) { alert("Geçerli bir telefon numarası giriniz"); return; }
    if (!kAddress) { alert("Adres zorunludur"); return; }
    if (!kBirthDate) { alert("Doğum tarihi zorunludur"); return; }
    if (birthDate > today) { alert("Doğum tarihi bugünden ileri olamaz"); return; }
    if (isNaN(kGender) || kGender === -1) { alert("Cinsiyet seçiniz"); return; }
    if (isNaN(kBloodType) || kBloodType === -1) { alert("Kan grubu seçiniz"); return; }
        
    const istek= await fetch(`https://localhost:1000/api/Patient`,
        {
            method:"POST",
            headers:{
               "Content-Type":"application/json"     
            },
            body: JSON.stringify(
                {
                    name:kName,
                    surname:kSurname,
                    address:kAddress,
                    phone:kPhone,
                    birthDate:kBirthDate,
                    gender:kGender,
                    bloodType:kBloodType,
                    tcKimlik:kTC
                }
            )
        }
    );
    if(!istek.ok)
        {
            const data= await istek.json();
            throw new Error(data.message ||"Bilinmeyen hata");
        }    
       alert("Hasta Kayıt başarılı");
    }
    catch(err)
    {
        alert(err.message)
    }
    finally
    {
    document.getElementById("kName").value = "";
    document.getElementById("kSurname").value = "";
    document.getElementById("kTC").value = "";
    document.getElementById("kAddress").value = "";
    document.getElementById("kPhone").value = "";
    document.getElementById("kBirthDate").value = "";
    document.getElementById("kGender").value = "-1";
    document.getElementById("kBloodType").value = "-1";
    }
    
});